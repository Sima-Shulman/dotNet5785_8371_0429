using BlApi;
using BO;
using Helpers;

namespace BlImplementation;

internal class CallImplementation : ICall
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddCall(BO.Call boCall)
    {
        DO.Call doCall = new((DO.CallType)boCall.CallType, boCall.Verbal_description, boCall.FullAddress, (double?)boCall.Latitude, boCall.Longitude, boCall.Opening_time, boCall.Max_finish_time);
        try
        {
            _dal.Call.Create(doCall);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
    }

    public void DeleteCall(int callId)
    {
        throw new NotImplementedException();
    }

    public BO.Call GetCallDetails(int callId)
    {

        var call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Student with ID={callId} does Not exist");
        return new()
        {
            Id = call.id,
            CallType = (BO.Enums)call.CallType,
            Verbal_description = (BO.Enums)call.Verbal_descruption,
            FullAddress = call.FullAddress,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            Opening_time = call.Opening_time,
            Max_finish_time = call.Max_finish_time,
            CallStatus =??
            AssignmentList =??
        };


    }


    public int[] GetCallQuantitiesByStatus()
    {
        var calls = _dal.Call.ReadAll();
        int[] callQuantities = new int[Enum.GetValues(typeof(CallStatus)).Length];

        var groupedCalls = calls
            .GroupBy(c => c.CalculateCallStatus()
            .ToDictionary(g => g.Key, g => g.Count());
        foreach (var entry in groupedCalls)
        {
            callQuantities[entry.Key] = entry.Value;
        }
        return callQuantities;
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

    public IEnumerable<BO.CallInList> GetCallsList(BO.Enums.CallFields? fieldFilter = null, object? filterValue = null, BO.Enums.CallFields? sortField = null)
    {
        IEnumerable<BO.CallInList?> calls = CallManager.ConvertToCallInList((IEnumerable<DO.Call>)_dal.Call.ReadAll());
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
        //if (sortField.HasValue)
        //{
        //    calls = calls.OrderBy(call =>
        //        sortField.Value switch
        //        {
        //            BO.Enums.CallFields.CallId => call.CallId,
        //            BO.Enums.CallFields.CallType => call.CallType,
        //            BO.Enums.CallFields.Opening_time => call.Opening_time,
        //            BO.Enums.CallFields.TimeLeft => call.TimeLeft,
        //            BO.Enums.CallFields.LastVolunteerName => call.LastVolunteerName,
        //            BO.Enums.CallFields.TotalTime => call.TotalTime,
        //            BO.Enums.CallFields.CallStatus => call.CallStatus,
        //            BO.Enums.CallFields.TotalAssignments => call.TotalAssignments,
        //            _ => call.CallId
        //        });
        //}
        //else
        //{
        //    calls = calls.OrderBy(call => call.CallId);
        //}

        if (fieldFilter.HasValue && filterValue != null)
        {
            calls = calls.Where(c => c.GetType().GetProperty(fieldFilter.ToString())?.GetValue(c)?.Equals(filterValue) == true);
        }

        calls = sortField.HasValue
            ? calls.OrderBy(c => c?.GetType().GetProperty(sortField.ToString())?.GetValue(c))
            : calls.OrderBy(c => c?.CallId);
        return calls!;
    }



    public IEnumerable<BO.ClosedCallInList> GetClosedCallsHandledByVolunteer(int volunteerId, BO.Enums.CallType? callTypeFilter = null, BO.Enums.CallFields? sortField = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.Enums.CallType? callTypeFilter = null, BO.Enums.CallFields? sortField = null)
    {
        throw new NotImplementedException();
    }

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

    public void UpdateCallDetails(Call call)
    {
        throw new NotImplementedException();
    }





}
