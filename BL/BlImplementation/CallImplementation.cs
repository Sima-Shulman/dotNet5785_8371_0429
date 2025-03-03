

using Helpers;
using System.Collections.Generic;
using System.Linq;


namespace BlImplementation;

internal class CallImplementation : BlApi.ICall
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    private bool isRequesterVolunteer;

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

    public IEnumerable<BO.CallInList> GetCallsList(BO.Enums.CallInListFields? fieldFilter = null, object? filterValue = null, BO.Enums.CallInListFields? sortField = null)
    {
        try
        {
            IEnumerable<BO.CallInList?> calls = CallManager.ConvertToCallInList((IEnumerable<DO.Call>)_dal.Call.ReadAll());
            if (fieldFilter.HasValue && filterValue is not null)
            {
                //foreach (var call in calls)
                //{
                //    Console.WriteLine(call);
                //    Console.WriteLine(call.GetType());
                //    Console.WriteLine(call.GetType().GetProperty(fieldFilter.ToString()));
                //    Console.WriteLine(call.GetType().GetProperty(fieldFilter.ToString())?.GetValue(call));
                //    Console.WriteLine(call.GetType().GetProperty(fieldFilter.ToString())?.GetValue(call)?.Equals((BO.Enums.CallStatus)filterValue) == true);

                //}

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

    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            DO.Call call = _dal.Call.Read(callId);
            //if (call is null)
            //    throw new BO.BlDoesNotExistException($"Call with ID={callId}does not exist!");
            var callAssignInLists = _dal.Assignment.ReadAll(a => a.CallId == callId)
                                               .Select(a => new BO.CallAssignInList
                                               {
                                                   VolunteerId = a.VolunteerId,
                                                   VolunteerFullName = _dal.Volunteer.Read(a.VolunteerId)?.FullName,
                                                   Start_time = a.Start_time,
                                                   End_time = a.End_time,
                                                   EndType = (BO.Enums.EndType?)a.EndType
                                               })
                                               .ToList();
            return new BO.Call
            {
                Id = call.Id,
                CallType = (BO.Enums.CallType)call.Call_type,
                Verbal_description = call.Verbal_description,
                FullAddress = call.Full_address,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
                Opening_time = call.Opening_time,
                Max_finish_time = (DateTime)call.Max_finish_time,
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
    public void UpdateCallDetails(BO.Call boCall)
    {
        try
        {
            /////////////צריך לבדוק את זה? כי בעיקרון זה העבודה של  הפונקציה אופדייט אבל אם לא התוכנית תקרוס כשננסה לגשת לאחד הערכים שלו
            ////וגם פה מה עדיף לבדוק כל פעם אם הכתובת שונתה אבל לשלוף שוב מהדאל או בכל מקרה לחשב קורדיננטות מחדש?
            /////פה
            var existingCall = _dal.Call.Read(boCall.Id) ?? throw new BO.BlDoesNotExistException($"Call with ID={boCall.Id} does not exist");
            //only if the user wants to update th address of the call
            if (boCall.FullAddress != "No Address")//להשוות עם הקיים
            {
                var (latitude, longitude) = Tools.GetCoordinatesFromAddress(boCall.FullAddress!);
                if (latitude is null || longitude is null)
                    throw new BO.BlInvalidFormatException($"Invalid address: {boCall.FullAddress}");
                boCall.Latitude = latitude;
                boCall.Longitude = longitude;
            }
            else
            {
                boCall.FullAddress = existingCall.Full_address;
                boCall.Latitude = existingCall.Latitude;
                boCall.Longitude = existingCall.Longitude;
            }
            CallManager.ValidateCall(boCall);
            DO.Call updatedCall = CallManager.ConvertBoCallToDoCall(boCall);
            _dal.Call.Update(updatedCall);
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

    public void DeleteCall(int callId)
    {
        try
        {
            DO.Call call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist");
            if (!_dal.Assignment.ReadAll(a => a!.CallId == callId).Any() || CallManager.CalculateCallStatus(call) != BO.Enums.CallStatus.opened)
                throw new BO.BlDeletionException($"Cannot delete call with ID={callId} as they are handling calls.");
            _dal.Call.Delete(callId);
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


    public void AddCall(BO.Call boCall)
    {
        try
        {
            var existingCall = _dal.Call.Read(boCall.Id);
            if (existingCall != null)
                throw new BO.BlAlreadyExistException($"Call with ID={boCall.Id} already exist");
            CallManager.ValidateCall(boCall);
            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(boCall.FullAddress!);
            if (latitude is null || longitude is null)
                throw new BO.BlInvalidFormatException($"Invalid address: {boCall.FullAddress}");
            boCall.Latitude = latitude;
            boCall.Longitude = longitude;
            DO.Call doCall = CallManager.ConvertBoCallToDoCall(boCall);
            _dal.Call.Create(doCall);
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

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsHandledByVolunteer(int volunteerId, BO.Enums.CallType? callTypeFilter = null, BO.Enums.ClosedCallInListFields? sortField = null)
    {
        //אני צריכה לבדוק שיש כזה וונולטיר ואם לא לזרוק שגיאה??
        //כן , מפה

        try
        {
            var closedCalls = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.End_time != null)
                         .Where(a => callTypeFilter is null || (BO.Enums.CallType)_dal.Call.Read(a.CallId).Call_type == callTypeFilter)
                     .Select(a =>
                     {
                         var call = _dal.Call.Read(a.CallId);
                         return new BO.ClosedCallInList
                         {
                             Id = call.Id,
                             CallType = (BO.Enums.CallType)call.Call_type,
                             FullAddress = call.Full_address,
                             Opening_time = call.Opening_time,
                             Start_time = a.Start_time,
                             End_time = a.End_time,
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

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.Enums.CallType? callTypeFilter = null, BO.Enums.OpenCallInListFields? sortField = null)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");
            var openCalls = _dal.Call.ReadAll()
                .Where(c =>
                (CallManager.CalculateCallStatus(c) == BO.Enums.CallStatus.opened || CallManager.CalculateCallStatus(c) == BO.Enums.CallStatus.opened_at_risk))
                .Select(c => new BO.OpenCallInList
                {
                    Id = c.Id,
                    CallType = (BO.Enums.CallType)c.Call_type,
                    Verbal_description = c.Verbal_description,
                    FullAddress = c.Full_address,
                    Start_time = c.Opening_time,
                    Max_finish_time = c.Max_finish_time,
                    CallDistance = Tools.CalculateDistance(volunteer.Latitude, volunteer.Longitude, c.Latitude, c.Longitude)
                });
            return sortField.HasValue
            ? openCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString())?.GetValue(c))
            : openCalls.OrderBy(c => c.Id);
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
    public void MarkCallCancellation(int volunteerId, int assignmentId)
    {
        try
        {
            var assignment = _dal.Assignment.Read(assignmentId);
            if (assignment == null)
                throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exist");
            var isRequesterManager = _dal.Volunteer?.Read(volunteerId).Role;
            if (isRequesterManager != DO.Role.manager || assignment.VolunteerId != volunteerId)
                throw new BO.BlUnauthorizedException("Requester does not have permission to cancel this assignment");
            if (assignment.End_time != null)
                throw new BO.BlDeletionException("Cannot cancel an assignment that has already been completed or expired");
            DO.Assignment copy = assignment with
            {
                End_time = _dal.Config.Clock,
                EndType = (DO.EndType)(isRequesterVolunteer ? BO.Enums.EndType.self_cancellation : BO.Enums.EndType.manager_cancellation),
            };
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




    public void MarkCallCompletion(int volunteerId, int assignmentId)
    {
        try
        {
            var assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exist.");
            if (assignment.VolunteerId != volunteerId)
                throw new BO.BlUnauthorizedException($"Volunteer with ID={volunteerId} is not authorized to complete this call.");
            if (assignment.End_time != null)
                throw new BO.BlDeletionException($"The assignment with ID={assignmentId} has already been completed or canceled.");
            DO.Assignment newAssignment = assignment with { End_time = _dal.Config.Clock, EndType = DO.EndType.was_treated };
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


    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        try
        {
            var call = GetCallDetails(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist.");
            if (call.CallStatus == BO.Enums.CallStatus.is_treated || call.CallStatus == BO.Enums.CallStatus.treated_at_risk)
                throw new BO.BlUnauthorizedException($"Call with ID={callId} is open already.You are not authorized to treat it.");
            if(call.CallStatus == BO.Enums.CallStatus.closed)
                throw new BO.BlUnauthorizedException($"Call with ID={callId} is closed already.You are not authorized to treat it.");
                        if (call.CallStatus == BO.Enums.CallStatus.expired)
                throw new BO.BlUnauthorizedException($"Call with ID={callId} is expired already.You are not authorized to treat it.");
            //var existingAssignments = _dal.Assignment.ReadAll(a => a?.CallId == callId);
            //if (existingAssignments.Any(a => a?.End_time == null))
            //    throw new BO.BlUnauthorizedException($"Call with ID={callId} is in treatment already.You are not authorized to treat it.");
            var newAssignment = new DO.Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                Start_time = _dal.Config.Clock,
                End_time = null,
                EndType = null
            };
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


}






//public int[] GetCallQuantitiesByStatus()
//{
//    var calls = _dal.Call.ReadAll();

//    // יצירת מערך בגודל של מספר הסטטוסים (מומלץ שיהיה גודל של Enum)
//    int[] callQuantities = new int[Enum.GetValues(typeof(CallStatus)).Length];

//    // סינון הקריאות על פי סטטוס וחישוב הכמויות
//    var groupedCalls = calls
//        .GroupBy(c => c.CalculateCallStatus())  // קיבוץ לפי סטטוס
//        .ToArray();  // ממיר את הקבוצות למערך

//    foreach (var group in groupedCalls)
//    {
//        // עדכון המערך עם כמות הקריאות לכל סטטוס
//        callQuantities[(int)group.Key] = group.Count();
//    }





//if (fieldFilter is not null)
//    calls = calls.Where(call =>
//        fieldFilter.Value switch
//        {
//            BO.Enums.CallInListFields.CallId => call.CallId.Equals(filterValue),
//            BO.Enums.CallInListFields.CallType => call.CallType.Equals(filterValue),
//            BO.Enums.CallInListFields.Opening_time => call.Opening_time.Equals(filterValue),
//            BO.Enums.CallInListFields.TimeLeft => call.TimeLeft.Equals(filterValue),
//            BO.Enums.CallInListFields.LastVolunteerName => call.LastVolunteerName.Contains((string)filterValue),
//            BO.Enums.CallInListFields.TotalTime => call.TotalTime.Equals(filterValue),
//            BO.Enums.CallInListFields.CallStatus => call.CallStatus.Equals(filterValue),
//            BO.Enums.CallInListFields.TotalAssignments => call.TotalAssignments.Equals(filterValue),
//            _ => true
//        });
//if (sortField.HasValue)
//{
//    calls = calls.OrderBy(call =>
//        sortField.Value switch
//        {
//            BO.Enums.CallInListFields.CallId => call.CallId,
//            BO.Enums.CallInListFields.CallType => call.CallType,
//            BO.Enums.CallInListFields.Opening_time => call.Opening_time,
//            BO.Enums.CallInListFields.TimeLeft => call.TimeLeft,
//            BO.Enums.CallInListFields.LastVolunteerName => call.LastVolunteerName,
//            BO.Enums.CallInListFields.TotalTime => call.TotalTime,
//            BO.Enums.CallInListFields.CallStatus => call.CallStatus,
//            BO.Enums.CallInListFields.TotalAssignments => call.TotalAssignments,
//            _ => call.CallId
//        });
//}
//else
//{
//    calls = calls.OrderBy(call => call.CallId);
//}


//if (fieldFilter.HasValue && filterValue != null)
//{
//    calls = calls.Where(call =>
//        fieldFilter.Value switch
//        {
//            BO.Enums.CallFields.CallId => call.CallId.Equals(filterValue),
//            BO.Enums.CallFields.CallType => call.CallType.Equals(filterValue),
//            BO.Enums.CallFields.Opening_time => call.Opening_time.Equals(filterValue),
//            BO.Enums.CallFields.TimeLeft => call.TimeLeft.Equals(filterValue),
//            BO.Enums.CallFields.LastVolunteerName => call.LastVolunteerName.Contains((string)filterValue),
//            BO.Enums.CallFields.TotalTime => call.TotalTime.Equals(filterValue),
//            BO.Enums.CallFields.CallStatus => call.CallStatus.Equals(filterValue),
//            BO.Enums.CallFields.TotalAssignments => call.TotalAssignments.Equals(filterValue),
//            _ => true
//        });
//}