

using DalApi;
using Helpers;
using System.Collections.Generic;
using System.Linq;


namespace BlImplementation;
/// <summary>
/// Provides business logic operations for managing calls in the system.
/// This includes operations such as retrieving, creating, updating, and deleting calls,
/// as well as assigning volunteers and changing call statuses.
/// </summary>

internal class CallImplementation : BlApi.ICall
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    /// <summary>
    /// Returns an array representing the number of calls per status.
    /// </summary>
    /// <returns>
    /// An array of integers where each index corresponds to a call status 
    /// and the value at that index represents the number of calls in that status.
    /// </returns>
    public int[] GetCallQuantitiesByStatus()
    {
        try
        {
            var calls = _dal.Call.ReadAll();
            int[] callQuantities = new int[Enum.GetValues(typeof(BO.Enums.CallStatus)).Length];

            var groupedCalls = calls
                .GroupBy(c => c.CalculateCallStatus())
                .ToDictionary(g => g.Key, g => g.Count());
            foreach (var entry in groupedCalls)
            {
                callQuantities[(int)entry.Key] = entry.Value;
            }
            return callQuantities;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Cannot access calls", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Unexpected error occurred.", ex);
        }
    }
    /// <summary>
    /// Returns a list of calls in a summarized form, with optional filtering and sorting.
    /// </summary>
    /// <param name="fieldFilter">Optional field to filter calls by.</param>
    /// <param name="filterValue">The value to filter by.</param>
    /// <param name="sortField">Optional field to sort the results by.</param>
    /// <returns>
    /// An IEnumerable of <see cref="BO.CallInList"/> containing the filtered and sorted calls.
    /// </returns>
    public IEnumerable<BO.CallInList> GetCallsList(BO.Enums.CallInListFields? fieldFilter = null, object? filterValue = null, BO.Enums.CallInListFields? sortField = null)
    {
        try
        {
            IEnumerable<BO.CallInList?> calls = CallManager.ConvertToCallInList((IEnumerable<DO.Call>)_dal.Call.ReadAll());
            if (fieldFilter.HasValue && filterValue is not null)
            {
                calls = calls.Where(c => c.GetType().GetProperty(fieldFilter.ToString())?.GetValue(c)?.Equals(filterValue) == true);
            }

            calls = sortField.HasValue
                ? calls.OrderBy(c => c?.GetType().GetProperty(sortField.ToString())?.GetValue(c))
                : calls.OrderBy(c => c?.CallId);
            return calls!;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Can not access calls", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Unexpected error occurred.", ex);
        }
    }

    /// <summary>
    /// Returns the details of a specific call by its ID.
    /// </summary>
    /// <param name="callId">The ID of the call to retrieve.</param>
    /// <returns>
    /// A <see cref="BO.Call"/> object representing the call with the given ID.
    /// </returns>
    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            DO.Call call = _dal.Call.Read(callId);
            if (call is null)
                throw new BO.BlDoesNotExistException($"Call with ID={callId}does not exist!");
            var callAssignInLists =
              (from a in _dal.Assignment.ReadAll(a => a.CallId == callId)
               select new BO.CallAssignInList
               {
                   VolunteerId = a.VolunteerId,
                   VolunteerFullName = _dal.Volunteer.Read(a.VolunteerId)?.FullName,
                   StartTime = a.StartTime,
                   EndTime = a.EndTime,
                   EndType = (BO.Enums.EndType?)a.EndType
               }).ToList();

            return new BO.Call
            {
                Id = call.Id,
                CallType = (BO.Enums.CallType)call.CallType,
                Description = call.Description,
                FullAddress = call.FullAddress,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
                OpeningTime = call.OpeningTime,
                MaxFinishTime = call.MaxFinishTime,
                CallStatus = call.CalculateCallStatus(),
                AssignmentsList = callAssignInLists,
            };
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Can not access calls", ex);
        }
        catch (BO.BlDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Unexpected error occurred.", ex);
        }
    }
    /// <summary>
    /// Updates the details of an existing call.
    /// </summary>
    /// <param name="boCall">The <see cref="BO.Call"/> object containing the updated details.</param>
    public void UpdateCallDetails(BO.Call boCall)
    {
        try
        {
            var existingCall = _dal.Call.Read(boCall.Id) ?? throw new BO.BlDoesNotExistException($"Call with ID={boCall.Id} does not exist");
            //only if the user wants to update th address of the call
            if (boCall.FullAddress != existingCall.FullAddress)
            {
                var (latitude, longitude) = Tools.GetCoordinatesFromAddress(boCall.FullAddress!);

                boCall.Latitude = latitude;
                boCall.Longitude = longitude;
            }
            else
            {
                boCall.FullAddress = existingCall.FullAddress;
                boCall.Latitude = existingCall.Latitude;
                boCall.Longitude = existingCall.Longitude;
            }
            CallManager.ValidateCall(boCall);
            DO.Call updatedCall = CallManager.ConvertBoCallToDoCall(boCall);
            _dal.Call.Update(updatedCall);
            CallManager.Observers.NotifyItemUpdated(updatedCall.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated(); //stage 5                                                    

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Can not access calls", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Unexpected error occurred.", ex);
        }
    }

    /// <summary>
    /// Deletes a specific call by its ID.
    /// </summary>
    /// <param name="callId">The ID of the call to delete.</param>
    public void DeleteCall(int callId)
    {
        try
        {
            DO.Call call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist");
            if (_dal.Assignment.ReadAll(a => a!.CallId == callId).Any() || CallManager.CalculateCallStatus(call) != BO.Enums.CallStatus.Opened)
                throw new BO.BlDeletionException($"Cannot delete call with ID={callId} as they are handling calls.");
            _dal.Call.Delete(callId);
            CallManager.Observers.NotifyListUpdated(); //stage 5                                                    
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist", ex);
        }
        catch (DO.DalDeletionImpossible ex)
        {
            throw new BO.BlDeletionException($"Cannot delete volunteer with ID={callId} as they are handling calls.", ex);
        }
        catch (BO.BlDeletionException ex)
        {
            throw new BO.BlDeletionException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Unexpected error occurred.", ex);
        }
    }

    /// <summary>
    /// Adds a new call to the system.
    /// </summary>
    /// <param name="boCall">The <see cref="BO.Call"/> object to add.</param>

    public void AddCall(BO.Call boCall)
    {
        try
        {
            var existingCall = _dal.Call.Read(boCall.Id);
            if (existingCall != null)
                throw new BO.BlAlreadyExistException($"Call with ID={boCall.Id} already exist");
            CallManager.ValidateCall(boCall);
            var (latitude, longitude) = Tools.GetCoordinatesFromAddress((string)boCall.FullAddress);
            boCall.Latitude = latitude;
            boCall.Longitude = longitude;
            DO.Call doCall = CallManager.ConvertBoCallToDoCall(boCall);
            _dal.Call.Create(doCall);
            NotifyVolunteersAboutNewCall(doCall);
            CallManager.Observers.NotifyListUpdated(); //stage 5                                                    
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistException($"Call with ID={boCall.Id} already exists", ex);
        }
        catch (BO.BlInvalidFormatException ex)
        {
            throw new BO.BlInvalidFormatException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Unexpected error occurred.", ex);
        }
    }
    /// <summary>
    /// Retrieves a list of closed calls handled by a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to retrieve closed calls for.</param>
    /// <param name="callTypeFilter">Optional filter to select specific call types.</param>
    /// <param name="sortField">Optional field to sort the calls by.</param>
    /// <returns>An IEnumerable of ClosedCallInList representing the closed calls handled by the volunteer.</returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsHandledByVolunteer(int volunteerId, BO.Enums.CallType? callTypeFilter = null, BO.Enums.ClosedCallInListFields? sortField = null)
    {
        try
        {
            var v = _dal.Volunteer.Read(volunteerId);
            if (v is null)
                throw new BO.BlDoesNotExistException($"Volunteer with ID = {volunteerId} does not exist!");
            var closedCalls = _dal.Assignment.ReadAll(a => a?.VolunteerId == volunteerId && a.EndTime != null)
                         .Where(a => callTypeFilter is null || (BO.Enums.CallType)_dal.Call.Read(a.CallId).CallType == callTypeFilter)
                     .Select(a =>
                     {
                         var call = _dal.Call.Read(a.CallId);
                         return new BO.ClosedCallInList
                         {
                             Id = call.Id,
                             CallType = (BO.Enums.CallType)call.CallType,
                             FullAddress = call.FullAddress,
                             OpeningTime = call.OpeningTime,
                             StartTime = a.StartTime,
                             EndTime = a.EndTime,
                             EndType = (BO.Enums.EndType)a.EndType
                         };
                     });
            return sortField.HasValue
                ? closedCalls.OrderBy(a => a.GetType().GetProperty(sortField.ToString())?.GetValue(a))
                : closedCalls.OrderBy(a => a.Id);

        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistException("Cannot access assignments", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Unexpected error occurred.", ex);
        }
    }
    /// <summary>
    /// Retrieves a list of open calls for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to retrieve open calls for.</param>
    /// <param name="callTypeFilter">Optional filter to select specific call types.</param>
    /// <param name="sortField">Optional field to sort the calls by.</param>
    /// <returns>An IEnumerable of OpenCallInList representing the open calls available for the volunteer.</returns>
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.Enums.CallType? callTypeFilter = null, BO.Enums.OpenCallInListFields? sortField = null)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");
            IEnumerable<BO.OpenCallInList> openCalls;
            if (volunteer.MaxDistance is null)
                openCalls = from c in _dal.Call.ReadAll(c => c.CalculateCallStatus() == BO.Enums.CallStatus.Opened ||
                                              c.CalculateCallStatus() == BO.Enums.CallStatus.OpenedAtRisk)
                            select new BO.OpenCallInList
                            {
                                Id = c.Id,
                                CallType = (BO.Enums.CallType)c.CallType,
                                Description = c.Description,
                                FullAddress = c.FullAddress,
                                StartTime = c.OpeningTime,
                                MaxFinishTime = c.MaxFinishTime,
                                CallDistance = Tools.CalculateDistance(volunteer.Latitude, volunteer.Longitude,
                                                       c.Latitude, c.Longitude, volunteer.DistanceTypes)
                            };

            else
                openCalls = from c in _dal.Call.ReadAll()
                            where (c.CalculateCallStatus() == BO.Enums.CallStatus.Opened ||
                                   c.CalculateCallStatus() == BO.Enums.CallStatus.OpenedAtRisk)
                                  && (volunteer.MaxDistance >= Tools.CalculateDistance(volunteer.Latitude, volunteer.Longitude,
                                                                                        c.Latitude, c.Longitude, volunteer.DistanceTypes))
                            select new BO.OpenCallInList
                            {
                                Id = c.Id,
                                CallType = (BO.Enums.CallType)c.CallType,
                                Description = c.Description,
                                FullAddress = c.FullAddress,
                                StartTime = c.OpeningTime,
                                MaxFinishTime = c.MaxFinishTime,
                                CallDistance = Tools.CalculateDistance(volunteer.Latitude, volunteer.Longitude,
                                                                       c.Latitude, c.Longitude, volunteer.DistanceTypes)
                            };

            return sortField.HasValue
            ? openCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString())?.GetValue(c))
            : openCalls.OrderBy(c => c.Id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.", ex);
        }
        catch (BO.BlDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Unexpected error occurred.", ex);
        }
    }
    /// <summary>
    /// Marks a call as canceled by a volunteer or manager.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer requesting the cancellation.</param>
    /// <param name="assignmentId">The ID of the assignment to cancel.</param>
    public void MarkCallCancellation(int volunteerId, int assignmentId)
    {
        try
        {
            var assignment = _dal.Assignment.Read(assignmentId);
            if (assignment == null)
                throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exist");
            bool isRequesterNotManager = _dal.Volunteer?.Read(volunteerId).Role != DO.Role.Manager;
            if (isRequesterNotManager && assignment.VolunteerId != volunteerId)
                throw new BO.BlUnauthorizedException("Requester does not have permission to cancel this assignment");
            if (assignment.EndTime != null)
                throw new BO.BlDeletionException("Cannot cancel an assignment that has already been completed or expired");
            DO.Assignment newAssignment = assignment with
            {
                EndTime = _dal.Config.Clock,
                EndType = (DO.EndType)(isRequesterNotManager ? BO.Enums.EndType.SelfCancellation : BO.Enums.EndType.ManagerCancellation),
            };
            _dal.Assignment.Update(newAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Unexpected error occurred.", ex);
        }
    }

    /// <summary>
    /// Marks a call as completed by a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer marking the call as completed.</param>
    /// <param name="assignmentId">The ID of the assignment to mark as completed.</param>
    public void MarkCallCompletion(int volunteerId, int assignmentId)
    {
        try
        {
            var assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exist.");
            if (assignment.VolunteerId != volunteerId)
                throw new BO.BlUnauthorizedException($"Volunteer with ID={volunteerId} is not authorized to complete this call.");
            if (assignment.EndType == DO.EndType.Expired || assignment.EndType == DO.EndType.WasTreated)
                throw new BO.BlDeletionException($"The assignment with ID={assignmentId} has already been completed or expired.");
            DO.Assignment newAssignment = assignment with { EndTime = _dal.Config.Clock, EndType = DO.EndType.WasTreated };
            _dal.Assignment.Update(newAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exist", ex);
        }
        catch (DO.DalDeletionImpossible ex)
        {
            throw new BO.BlDeletionException($"The assignment with ID={assignmentId} has already been completed or canceled.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Unexpected error occurred.", ex);
        }
    }

    /// <summary>
    /// Selects a call for treatment by a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer selecting the call for treatment.</param>
    /// <param name="callId">The ID of the call to be selected for treatment.</param>
    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        try
        {
            var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.EndTime is null);
            if (assignments.Any())
                throw new BO.BlUnauthorizedException($"volunteer with ID {volunteerId} cannot select a new call for treatment since he is treating another call now.");
            var call = GetCallDetails(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist.");
            if (call.CallStatus == BO.Enums.CallStatus.InTreatment || call.CallStatus == BO.Enums.CallStatus.InTreatmentAtRisk)
                throw new BO.BlUnauthorizedException($"Call with ID={callId} is open already.You are not authorized to treat it.");
            if (call.CallStatus == BO.Enums.CallStatus.Closed)
                throw new BO.BlUnauthorizedException($"Call with ID={callId} is closed already.You are not authorized to treat it.");
            if (call.CallStatus == BO.Enums.CallStatus.Expired)
                throw new BO.BlUnauthorizedException($"Call with ID={callId} is expired already.You are not authorized to treat it.");
            //var existingAssignments = _dal.Assignment.ReadAll(a => a?.CallId == callId);
            //if (existingAssignments.Any(a => a?.End_time == null))
            //    throw new BO.BlUnauthorizedException($"Call with ID={callId} is in treatment already.You are not authorized to treat it.");
            var newAssignment = new DO.Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                StartTime = _dal.Config.Clock,
                EndTime = null,
                EndType = null
            };
            var volunteer = _dal.Volunteer.Read(volunteerId);
            if (volunteer is null)
                throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");
            ///צריך לבדוק שהקריאה לא מידי רחוקה??
            if (volunteer.MaxDistance < Tools.CalculateDistance(volunteer.Latitude, volunteer.Longitude, call.Latitude, call.Longitude, volunteer.DistanceTypes))
                throw new BO.BlUnauthorizedException($"Volunteer with ID={volunteerId} is not authorized to treat call with ID={callId} because it is too far.");
            _dal.Assignment.Create(newAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist", ex);
        }
        catch (BO.BlUnauthorizedException ex)
        {
            throw new BO.BlUnauthorizedException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Unexpected error occurred.", ex);
        }
    }
    private void NotifyVolunteersAboutNewCall(DO.Call call)
    {
        try
        {
            var volunteers = _dal.Volunteer.ReadAll(v =>
                v!.IsActive &&
                v.Latitude != null && v.Longitude != null &&
                v.Email != null);

            foreach (var volunteer in volunteers)
            {
                double distance = Tools.CalculateDistance(
                    volunteer?.Latitude!.Value,
                    volunteer?.Longitude!.Value,
                    call.Latitude,
                    call.Longitude,
                    volunteer.DistanceTypes);

                if (volunteer.MaxDistance == null || distance <= volunteer.MaxDistance)
                {
                    string subject = "New Call Available Near You!";
                    string body = BuildEmailBody(volunteer, call, distance);
                    EmailHelper.SendEmail(volunteer.Email, subject, body);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to notify volunteers: " + ex.Message);
        }
    }
    private string BuildEmailBody(DO.Volunteer volunteer, DO.Call call, double distance)
    {
        string message = call.CallType switch
        {
            DO.CallType.CarAccident => "This is a car accident emergency call.",
            DO.CallType.Transportation => "This is a transportation assistance call.",
            DO.CallType.SearchAndRescue => "This is an sear and rescue emergency call.",
            DO.CallType.VehicleBreakdown => "This is an vehicle breakdown support call.",
            _ => "A new call has been opened."
        };

        return
            $"Hello {volunteer.FullName},\n\n" +
            $"{message}\n\n" +
            $"Call details:\n" +
            $"- ID: {call.Id}\n" +
            $"- Description: {call.Description}\n" +
            $"- Address: {call.FullAddress}\n" +
            $"- Opening Time: {call.OpeningTime}\n" +
            $"- Max Finish Time: {call.MaxFinishTime}\n" +
            $"- Distance: {distance:F2} km\n\n" +
            $"Please log in to the system to accept the call.\n\n" +
            $"Thank you for your service!";
    }



    #region Stage 5
    public void AddObserver(Action listObserver) =>
    CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5
}