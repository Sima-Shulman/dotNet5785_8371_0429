
using DalApi;
using static BO.Enums;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get;

    public static BO.Enums.CallStatus CalculateCallStatus(this DO.Call call)
    {
        List<DO.Assignment?> assignments = s_dal.Assignment.ReadAll(a => a?.CallId == call.Id).ToList();
        var lastAssignment = assignments.LastOrDefault(a => a.CallId == call.Id);
        // אם עבר זמן מקסימלי לסיום הקריאה
        if (call.Max_finish_time < DateTime.Now)
            return BO.Enums.CallStatus.expired;
        // אם הקריאה פתוחה ומסיימת בזמן הסיכון
        if ((DateTime.Now - call.Opening_time).TotalHours > s_dal.Config.RiskRange.TotalHours)
            return BO.Enums.CallStatus.opened_at_risk;
        // אם הקריאה בטיפול
        if (lastAssignment != null)
            //בטיפול בסיכון
            if ((DateTime.Now - lastAssignment?.Start_time) > s_dal.Config.RiskRange)
                return BO.Enums.CallStatus.treated_at_risk;
            //רק בטיפול      
            else
                return BO.Enums.CallStatus.is_treated;
        if (lastAssignment is not null && lastAssignment.End_time.HasValue)
            return BO.Enums.CallStatus.closed;
        return BO.Enums.CallStatus.opened;
    }
    public static IEnumerable<BO.CallInList> ConvertToCallInList(IEnumerable<DO.Call> calls)
    {
        return calls.Select(call =>
        {
            List<DO.Assignment?> assignments = s_dal.Assignment.ReadAll(a => a?.CallId == call.Id).ToList();
            var lastAssignment = assignments.LastOrDefault(a => a.CallId == call.Id);
            var lastVolunteerName = lastAssignment is not null ? s_dal.Volunteer.Read(lastAssignment.VolunteerId).FullName : null;
            TimeSpan? timeLeft = call.Max_finish_time > DateTime.Now ? call.Max_finish_time - DateTime.Now : null;
            BO.Enums.CallStatus callStatus = CalculateCallStatus(call);
            TimeSpan? totalTime = callStatus == BO.Enums.CallStatus.closed ? (call.Max_finish_time - call.Opening_time) : null;
            return new BO.CallInList
            {
                Id = lastAssignment?.Id,
                CallId = call.Id,
                CallType = (BO.Enums.CallType)call.Call_type,
                Opening_time = call.Opening_time,
                TimeLeft = timeLeft,
                LastVolunteerName = lastVolunteerName,
                TotalTime = totalTime,
                CallStatus = callStatus,
                TotalAssignments = assignments.Count()
            };
        }).ToList();

    }

    public static void ValidateCall(BO.Call call)
    {
        if (call is null)
            throw new BO.BlInvalidFormatException("Call cannot be null!");

        // בדיקת תקינות הכתובת
        if (string.IsNullOrWhiteSpace(call.FullAddress))
            throw new BO.BlInvalidFormatException("Invalid address!");

        if (!string.IsNullOrWhiteSpace(call.Verbal_description))
            throw new BO.BlInvalidFormatException("Invalid description!");

        // בדיקת זמני קריאה
        if (call.Opening_time == default)
            throw new BO.BlInvalidFormatException("Invalid opening time!");

        if (call.Max_finish_time != default && call.Max_finish_time <= call.Opening_time)
            throw new BO.BlInvalidFormatException("Invalid max finish time! Finish time has to be bigger than opening time.");

        // בדיקת מזהה מספרי
        if (call.Id <= 0)
            throw new BO.BlInvalidFormatException("Invalid id number! Id number has to be positive.");

        // בדיקת ENUM לשדות שאינם מספרים
        if (!Enum.IsDefined(typeof(CallStatus), call.CallStatus))
            throw new BO.BlInvalidFormatException("סInvalid status!");

        if (!Enum.IsDefined(typeof(DO.CallType), call.CallType))
            throw new BO.BlInvalidFormatException("Invalid call type!");

        // בדיקת רשימה
        if (call.AssignmentsList != null && call.AssignmentsList.Exists(a => a == null))
            throw new BO.BlInvalidFormatException("Invalid assignments list!");
    }

    public static DO.Call ConvertBoCallToDoCall(BO.Call call)
    {
        return new DO.Call
        {
            Id = call.Id,
            Call_type = (DO.CallType)call.CallType,
            Verbal_description = call.Verbal_description,
            Full_address = call.FullAddress,
            Latitude = call.Latitude ?? 0.0,
            Longitude = call.Longitude ?? 0.0,
            Opening_time = call.Opening_time,
            Max_finish_time = call.Max_finish_time,
        };
    }
}
