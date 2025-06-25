
using DalApi;
using static BO.Enums;

namespace Helpers;

/// <summary>
/// A static helper class that provides various operations related to Calls,
/// such as calculating their status, converting data objects, validation, and periodic updates.
/// </summary>
internal static class CallManager
{
    private static IDal s_dal = Factory.Get;
    internal static ObserverManager Observers = new(); //stage 5 

    /// <summary>
    /// Calculates the status of a given call based on its assignments and timing.
    /// </summary>
    /// <param name="call">The call to evaluate.</param>
    /// <returns>The calculated status of the call.</returns>
    public static BO.Enums.CallStatus CalculateCallStatus(this DO.Call call)
    {
        List<DO.Assignment?> assignments;
        lock (AdminManager.BlMutex)//stage 7
            assignments = s_dal.Assignment.ReadAll(a => a?.CallId == call.Id)
          .OrderByDescending(a => a?.StartTime)
          .ToList();

        if (assignments.Any()) // there IS an assignment to this call  
        {
            var lastAssignment = assignments.First();
            if (lastAssignment!.EndTime is not null) // if end time is NOT null  
            {
                if ((lastAssignment.EndType == DO.EndType.SelfCancellation) || (lastAssignment.EndType == DO.EndType.ManagerCancellation) && (lastAssignment.StartTime <= s_dal.Config.Clock)) // if the call was cancelled  
                {
                    if (call.MaxFinishTime.HasValue && (call.MaxFinishTime.Value - s_dal.Config.Clock) <= s_dal.Config.RiskRange) // if the call is in risk  
                        return BO.Enums.CallStatus.OpenedAtRisk; // OpenedAtRisk  
                    else
                        return BO.Enums.CallStatus.Opened; // Opened  
                }
                else // if the end time is not null but the call is not canceled ( - the call was ended in some way)
                {
                    if (lastAssignment.EndType == DO.EndType.Expired || call.MaxFinishTime < s_dal.Config.Clock) // Max finish time was over  
                        return BO.Enums.CallStatus.Expired; // Expired  
                    else
                        return BO.Enums.CallStatus.Closed; // Closed  
                }
            }
            else // end time IS null (- the assignment is the first one)  
            {
                if (call.MaxFinishTime.HasValue)
                {
                    if (call.MaxFinishTime <= s_dal.Config.Clock)
                        return BO.Enums.CallStatus.InTreatmentAtRisk; // InTreatmentAtRisk  

                    if ((call.MaxFinishTime.Value - s_dal.Config.Clock) <= s_dal.Config.RiskRange)
                        return BO.Enums.CallStatus.InTreatment; // InTreatment  
                }
                else
                    return BO.Enums.CallStatus.InTreatment;
            }
        }
        else // there are no assignments to this call  
        {
            if (call.MaxFinishTime.HasValue) // if there is a max finish time  
            {
                if (call.MaxFinishTime <= s_dal.Config.Clock)
                    return BO.Enums.CallStatus.Expired; // Expired  
                else if (call.MaxFinishTime - s_dal.Config.Clock <= s_dal.Config.RiskRange)
                    return BO.Enums.CallStatus.OpenedAtRisk; // OpenedAtRisk  
                else
                    return BO.Enums.CallStatus.Opened; // Opened  
            }
            else
            {
                return BO.Enums.CallStatus.Opened; // Opened  
            }
        }
        // Ensure all code paths return a value
        return BO.Enums.CallStatus.None; // Default fallback
    }
    /// <summary>
    /// Converts a list of DO.Call objects to a list of BO.CallInList objects.
    /// </summary>
    /// <param name="calls">The collection of DO.Call objects.</param>
    /// <returns>A list of BO.CallInList with enriched information.</returns>
    public static IEnumerable<BO.CallInList> ConvertToCallInList(IEnumerable<DO.Call> calls)
    {
        return calls.Select(call =>
        {
            lock (AdminManager.BlMutex)//stage 7
            {
                List<DO.Assignment?> assignments = s_dal.Assignment.ReadAll(a => a?.CallId == call.Id).ToList();
                var lastAssignment = assignments.LastOrDefault(a => a?.CallId == call.Id);
                var lastVolunteerName = lastAssignment is not null ? s_dal.Volunteer.Read(lastAssignment.VolunteerId)?.FullName : null;
                TimeSpan? timeLeft = call.MaxFinishTime > DateTime.Now ? call.MaxFinishTime - DateTime.Now : null;
                BO.Enums.CallStatus callStatus = CalculateCallStatus(call);
                TimeSpan? totalTime = callStatus == BO.Enums.CallStatus.Closed ? (call.MaxFinishTime - call.OpeningTime) : null;
                return new BO.CallInList
                {
                    AssignmentId = lastAssignment?.Id,
                    CallId = call.Id,
                    CallType = (BO.Enums.CallType)call.CallType,
                    OpeningTime = call.OpeningTime,
                    TimeLeft = timeLeft,
                    LastVolunteerName = lastVolunteerName,
                    TotalTime = totalTime,
                    CallStatus = callStatus,
                    TotalAssignments = assignments.Count()
                };
            }
        }).ToList();

    }
    /// <summary>
    /// Validates the format and content of a BO.Call object.
    /// </summary>
    /// <param name="call">The BO.Call to validate.</param>
    /// <exception cref="BO.BlInvalidFormatException">Thrown when any validation check fails.</exception>
    public static void ValidateCall(BO.Call call)
    {
        if (call is null)
            throw new BO.BlInvalidFormatException("Call cannot be null!");
        if (string.IsNullOrWhiteSpace(call.FullAddress))
            throw new BO.BlInvalidFormatException("Invalid address!");
        if (string.IsNullOrWhiteSpace(call.Description))
            throw new BO.BlInvalidFormatException("Invalid description!");
        if (call.OpeningTime == default)
            throw new BO.BlInvalidFormatException("Invalid opening time!");
        if (call.MaxFinishTime != default && call.MaxFinishTime <= call.OpeningTime)
            throw new BO.BlInvalidFormatException("Invalid max finish time! Finish time has to be bigger than opening time.");
        if (call.Id < 0)
            throw new BO.BlInvalidFormatException("Invalid id number! Id number has to be positive.");
        if (!Enum.IsDefined(typeof(CallStatus), call.CallStatus))
            throw new BO.BlInvalidFormatException("סInvalid status!");
        if (!Enum.IsDefined(typeof(BO.Enums.CallType), call.CallType))
            throw new BO.BlInvalidFormatException("Invalid call type!");
        if (call.AssignmentsList != null && call.AssignmentsList.Exists(a => a == null))
            throw new BO.BlInvalidFormatException("Invalid assignments list!");
    }
    /// <summary>
    /// Converts a BO.Call object into a DO.Call object.
    /// </summary>
    /// <param name="call">The BO.Call object to convert.</param>
    /// <returns>A DO.Call representation of the call.</returns>
    public static DO.Call ConvertBoCallToDoCall(BO.Call call)
    {
        return new DO.Call
        {
            Id = call.Id,
            CallType = (DO.CallType)call.CallType,
            Description = call.Description,
            FullAddress = call.FullAddress,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpeningTime = call.OpeningTime,
            MaxFinishTime = call.MaxFinishTime,
        };
    }
    /// <summary>
    /// Periodically updates calls that should be marked as expired based on a time window.
    /// </summary>
    /// <param name="oldClock">The previous timestamp before update.</param>
    /// <param name="newClock">The current timestamp after update.</param>
    internal static void PeriodicCallUpdates(DateTime oldClock, DateTime newClock)
    {
        lock (AdminManager.BlMutex)//stage 7
        {
            List<DO.Call?> expiredCalls = s_dal.Call.ReadAll(c => c?.MaxFinishTime > newClock).ToList();

            expiredCalls.ForEach(call =>
            {
                List<DO.Assignment?> assignments = s_dal.Assignment.ReadAll(a => a?.CallId == call?.Id).ToList();
                if (!assignments.Any())//the call is expired and there are no assignments to it
                    s_dal.Assignment.Create(new DO.Assignment(
                    CallId: call!.Id,
                    VolunteerId: 0,
                    StartTime: AdminManager.Now,
                    EndTime: AdminManager.Now,
                    EndType: (DO.EndType)BO.Enums.EndType.Expired
                ));
            });
        }
    }
}
