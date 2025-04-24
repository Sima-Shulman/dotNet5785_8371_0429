
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
    /// <summary>
    /// Calculates the status of a given call based on its assignments and timing.
    /// </summary>
    /// <param name="call">The call to evaluate.</param>
    /// <returns>The calculated status of the call.</returns>
    public static BO.Enums.CallStatus CalculateCallStatus(this DO.Call call)
    {
        var assignments = s_dal.Assignment.ReadAll(a => a.CallId == call.Id)
                .OrderByDescending(a => a.StartTime)
                .ToList();

        if (assignments.Any())//there is an assignment to this call
        {
            var lastAssignment = assignments.First();
            if (lastAssignment.EndTime is null && lastAssignment.StartTime <= s_dal.Config.Clock)
            {
                if (call.MaxFinishTime.HasValue && (call.MaxFinishTime.Value - s_dal.Config.Clock) <= s_dal.Config.RiskRange)
                    return BO.Enums.CallStatus.InTreatmentAtRisk; // InRiskTreatment

                return BO.Enums.CallStatus.InTreatment; // InTreatment
            }
            if (lastAssignment.EndType.HasValue)
            {
                if (lastAssignment.EndType == DO.EndType.Expired)
                    return BO.Enums.CallStatus.Expired; // Expired

                return BO.Enums.CallStatus.Closed; // Closed (handled or canceled)
            }
        }
        if (call.MaxFinishTime.HasValue)
        {
            if (call.MaxFinishTime <= s_dal.Config.Clock)
                return BO.Enums.CallStatus.Expired; // Expired

            if ((call.MaxFinishTime.Value - s_dal.Config.Clock) <= s_dal.Config.RiskRange)
                return BO.Enums.CallStatus.OpenedAtRisk; // OpenInRisk
        }
        return BO.Enums.CallStatus.Opened;
    }

    //        if (lastAssignment.EndType is null)//means that that treatment is not completed.
    //        {
    //            if (call.Max_finish_time - s_dal.Config.Clock < TimeSpan.Zero)//if the dead-line is over.
    //                return BO.Enums.CallStatus.expired;
    //            else if (call.Max_finish_time - s_dal.Config.Clock <= s_dal.Config.RiskRange)//if the call is in the risk range.
    //                return BO.Enums.CallStatus.treated_at_risk;
    //            else return BO.Enums.CallStatus.is_treated;//not in risk and not expired.
    //        }
    //        else if (lastAssignment.EndType == DO.EndType.was_treated)//means the treatment was completed.
    //            return BO.Enums.CallStatus.closed;
    //        else//the treatment wasn't completed.
    //        {
    //            if (call.Max_finish_time - s_dal.Config.Clock < TimeSpan.Zero)
    //                return BO.Enums.CallStatus.expired;//if the dead-line is over.
    //            else if (call.Max_finish_time - s_dal.Config.Clock <= s_dal.Config.RiskRange)
    //                return BO.Enums.CallStatus.opened_at_risk;//if it was not completed but the call is in the risk range.
    //            else return BO.Enums.CallStatus.opened;//it was not completed but still not expired and not in risk.
    //        }
    //    }
    //    else// there is no assignments to this call
    //    {
    //        if (call.Max_finish_time - s_dal.Config.Clock < TimeSpan.Zero)
    //            return BO.Enums.CallStatus.expired;//if the dead-line is over.
    //        if (call.Max_finish_time - s_dal.Config.Clock <= s_dal.Config.RiskRange)//if the call is in the risk range.
    //            return BO.Enums.CallStatus.opened_at_risk;
    //        else return BO.Enums.CallStatus.opened;//it still not expired and not in risk.
    //    }
    //}


    /// <summary>
    /// Converts a list of DO.Call objects to a list of BO.CallInList objects.
    /// </summary>
    /// <param name="calls">The collection of DO.Call objects.</param>
    /// <returns>A list of BO.CallInList with enriched information.</returns>
    public static IEnumerable<BO.CallInList> ConvertToCallInList(IEnumerable<DO.Call> calls)
    {
        return calls.Select(call =>
        {
            List<DO.Assignment?> assignments = s_dal.Assignment.ReadAll(a => a?.CallId == call.Id).ToList();
            var lastAssignment = assignments.LastOrDefault(a => a.CallId == call.Id);
            var lastVolunteerName = lastAssignment is not null ? s_dal.Volunteer.Read(lastAssignment.VolunteerId).FullName : null;
            TimeSpan? timeLeft = call.MaxFinishTime > DateTime.Now ? call.MaxFinishTime - DateTime.Now : null;
            BO.Enums.CallStatus callStatus = CalculateCallStatus(call);
            TimeSpan? totalTime = callStatus == BO.Enums.CallStatus.Closed ? (call.MaxFinishTime - call.OpeningTime) : null;
            return new BO.CallInList
            {
                Id = lastAssignment?.Id,
                CallId = call.Id,
                CallType = (BO.Enums.CallType)call.CallType,
                OpeningTime = call.OpeningTime,
                TimeLeft = timeLeft,
                LastVolunteerName = lastVolunteerName,
                TotalTime = totalTime,
                CallStatus = callStatus,
                TotalAssignments = assignments.Count()
            };
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

        // בדיקת תקינות הכתובת
        if (string.IsNullOrWhiteSpace(call.FullAddress))
            throw new BO.BlInvalidFormatException("Invalid address!");

        if (string.IsNullOrWhiteSpace(call.Description))
            throw new BO.BlInvalidFormatException("Invalid description!");

        // בדיקת זמני קריאה
        if (call.OpeningTime == default)
            throw new BO.BlInvalidFormatException("Invalid opening time!");

        if (call.MaxFinishTime != default && call.MaxFinishTime <= call.OpeningTime)
            throw new BO.BlInvalidFormatException("Invalid max finish time! Finish time has to be bigger than opening time.");

        // בדיקת מזהה מספרי
        if (call.Id < 0)
            throw new BO.BlInvalidFormatException("Invalid id number! Id number has to be positive.");

        // בדיקת ENUM לשדות שאינם מספרים
        if (!Enum.IsDefined(typeof(CallStatus), call.CallStatus))
            throw new BO.BlInvalidFormatException("סInvalid status!");

        if (!Enum.IsDefined(typeof(BO.Enums.CallType), call.CallType))
            throw new BO.BlInvalidFormatException("Invalid call type!");

        // בדיקת רשימה
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
            Latitude = call.Latitude /*?? 0.0*/,
            Longitude = call.Longitude /*?? 0.0*/,
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
        List<DO.Call> expiredCalls = s_dal.Call.ReadAll(c => c.MaxFinishTime > newClock).ToList();

        expiredCalls.ForEach(call =>
        {
            List<DO.Assignment> assignments = s_dal.Assignment.ReadAll(a => a.CallId == call.Id).ToList();
            if (!assignments.Any())
                s_dal.Assignment.Create(new DO.Assignment(
                CallId: call.Id,
                VolunteerId: 0,
                StartTime: ClockManager.Now,
                EndTime: ClockManager.Now,
                EndType: (DO.EndType)BO.Enums.EndType.Expired
            ));
            List<DO.Assignment> assignmentsWithNull = s_dal.Assignment.ReadAll(a => a.CallId == call.Id && a.EndType is null).ToList();
            if (assignmentsWithNull.Any())
                assignments.ForEach(assignment =>
                    s_dal.Assignment.Update(assignment with
                    {
                        EndTime = ClockManager.Now,
                        EndType = (DO.EndType)BO.Enums.EndType.Expired
                    }));
        });

    }
}
