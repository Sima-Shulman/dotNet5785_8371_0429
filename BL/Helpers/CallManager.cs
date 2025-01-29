
using DalApi;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get;

    public static BO.Enums.CallStatus CalculateCallStatus(this DO.Call call)
    {
        var assignments = GetAssignmentsByCallId(call.Id);

        if (assignments == null || !assignments.Any())
        {
            var riskTimeWindow = TimeSpan.FromMinutes(s_dal.Config.RiskRange);
            if (call.Max_finish_time - DateTime.Now <= riskTimeWindow)
            {
                return BO.Enums.CallStatus.opened_at_risk;
            }
            return BO.Enums.CallStatus.opened;
        }
        var assignment = assignments.FirstOrDefault();
        if (assignment != null && assignment.AssignmentStatus == BO.Enums.AssignmentStatus.InProgress)
        {
            return BO.Enums.CallStatus.is_treated;
        }
        if (DateTime.Now > call.Max_finish_time)
        {
            return BO.Enums.CallStatus.expired;
        }
        if (assignment != null && assignment.AssignmentStatus == BO.Enums.AssignmentStatus.Completed)
        {
            return BO.Enums.CallStatus.closed;
        }
        return BO.Enums.CallStatus.opened;
    }

    //public static List<Assignment> GetAssignmentsByCallId(int callId)
    //{
    //    List<Assignment?> assignments = s_dal.Assignment.ReadAll(a => a?.CallId == callId).ToList();
    //    return assignments;
    //}


    //private static BO.Enums.CallStatus GetCallStatus(DO.Call call, DO.Assignment? lastAssignment)
    //{


    //    if (call.Max_finish_time < DateTime.Now)
    //        return BO.Enums.CallStatus.expired;

    //    if ((DateTime.Now - call.Opening_time).TotalHours > 1)
    //        return BO.Enums.CallStatus.opened_at_risk;

    //    //double minutesLeft = (call.Max_finish_time - DateTime.Now).TotalMinutes;
    //    //TimeSpan riskThreshold =  s_dal.Config.RiskRange;

    //    //if (lastAssignment == null)
    //    //{
    //    //    return minutesLeft <= riskThreshold
    //    //        ? BO.Enums.CallStatus.opened_at_risk
    //    //        : BO.Enums.CallStatus.opened;
    //    //}

    //    //return minutesLeft <= riskThreshold
    //    //    ? BO.Enums.CallStatus.treated_at_risk
    //    //    : BO.Enums.CallStatus.is_treated;


    //}




    private static BO.Enums.CallStatus GetCallStatus(DO.Call call, DO.Assignment? lastAssignment)
    {
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
            var assignments = s_dal.Assignment.ReadAll().Where(a => a.CallId == call.Id).ToList();
            var lastAssignment = assignments.OrderByDescending(a => a?.CallId).LastOrDefault();   //FirstOrDefault;  //לפי מה הוא אחרון??
            var lastVolunteerName = lastAssignment is not null ? s_dal.Volunteer.Read(lastAssignment.VolunteerId).FullName : null;
            TimeSpan? timeLeft = call.Max_finish_time > DateTime.Now ? call.Max_finish_time - DateTime.Now : null;
            BO.Enums.CallStatus callStatus = GetCallStatus(call, lastAssignment);
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
}
