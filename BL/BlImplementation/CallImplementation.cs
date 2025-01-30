using BlApi;
using DalApi;
using Helpers;
using System.Collections.Generic;
using System.Linq;

namespace BlImplementation;

internal class CallImplementation : ICall
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public int[] GetCallQuantitiesByStatus()
    {
        var calls = _dal.Call.ReadAll();
        int[] callQuantities = new int[Enum.GetValues(typeof(BO.Enums.CallStatus)).Length];

        var groupedCalls = calls
            .GroupBy(c => c.CalculateCallStatus())
            .ToDictionary(g => g.Key, g => g.Count());
        foreach (var entry in groupedCalls)
        {
            callQuantities[entry.Key] = entry.Value;
        }
        return callQuantities;
    }

    public IEnumerable<BO.CallInList> GetCallsList(BO.Enums.CallInListFields? fieldFilter = null, object? filterValue = null, BO.Enums.CallInListFields? sortField = null)
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
    }

    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            DO.Call call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Student with ID={callId} does Not exist");
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
                CallStatus = CallManager.CalculateCallStatus(call),
                AssignmentsList = callAssignInLists,
            };
        }
        catch (Exception ex)
        {
            throw new Exception("dddd");
        }


    }

    public void UpdateCallDetails(BO.Call boCall)
    {
        try
        {
            var existingCall = _dal.Call.Read(boCall.Id) ?? throw new BO.BlDoesNotExistException($"Call with ID={boCall.Id} does not exist");
            CallManager.ValidateCall(boCall);
            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(boCall.FullAddress!);
            if (latitude is null || longitude is null)
                throw new BO.GeolocationNotFoundException($"Invalid address: {boCall.FullAddress}");
            boCall.Latitude = latitude;
            boCall.Longitude = longitude;
            DO.Call updatedCall = CallManager.ConvertBoCallToDoCall(boCall);
            _dal.Call.Update(updatedCall);
        }
        catch (Exception ex) { throw new Exception("aa"); }
    }

    public void DeleteCall(int callId)
    {
        try
        {
            DO.Call call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist");
            if (!_dal.Assignment.ReadAll(a => a!.CallId == callId).Any() || CallManager.CalculateCallStatus(call) != BO.Enums.CallStatus.opened)
                throw new BO.BlInvalidOperationException($"Cannot delete volunteer with ID={callId} as they are handling calls.");
            _dal.Call.Delete(callId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist", ex);
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
                throw new BO.GeolocationNotFoundException($"Invalid address: {boCall.FullAddress}");
            boCall.Latitude = latitude;
            boCall.Longitude = longitude;
            DO.Call doCall = CallManager.ConvertBoCallToDoCall(boCall);
            _dal.Call.Create(doCall);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Call with ID={boCall.Id} already exists", ex);
        }
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsHandledByVolunteer(int volunteerId, BO.Enums.CallType? callTypeFilter = null, BO.Enums.ClosedCallInListFields? sortField = null)
    {
        try
        {
            var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.End_time != null)
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
                ? assignments.OrderBy(a => a.GetType().GetProperty(sortField.ToString())?.GetValue(a)) 
                : assignments.OrderBy(a => a.Id);

        }
        catch (Exception ex) { throw new Exception("hjg"); }
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.Enums.CallType? callTypeFilter = null, BO.Enums.CallFields? sortField = null)
    {
        var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");
        var openCalls = _dal.Call.ReadAll()
            .Where(c =>
            (CallManager.CalculateCallStatus(c) == BO.Enums.CallStatus.opened || CallManager.CalculateCallStatus(c) == BO.Enums.CallStatus.opened_at_risk))
            .Select(c => new BO.OpenCallInList
            {
                Id = volunteerId, 
                CallType = (BO.Enums.CallType)c.Call_type, 
                Verbal_description = c.Verbal_description,
                FullAddress = c.Full_address,
                Start_time = c.Opening_time, // זמן פתיחת הקריאה
                Max_finish_time = c.Max_finish_time, // זמן סיום משוער
                CallDistance = Tools.CalculateDistance(volunteer.Latitude, volunteer.Longitude, c.Latitude, c.Longitude)
            });

        // שלב 4: סינון לפי סוג הקריאה (אם לא null)
        return sortField.HasValue
        ? openCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString())?.GetValue(c))
        : openCalls.OrderBy(c => c.Id);
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





    public void MarkCallCancellation(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void MarkCallCompletion(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }



}
