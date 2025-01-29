namespace BlApi
{
    public interface ICall
    {
        int[] GetCallQuantitiesByStatus();
        IEnumerable<BO.CallInList> GetCallsList(BO.Enums.CallFields? fieldFilter = null, object? filterValue = null, BO.Enums.CallFields? sortField = null);
        BO.Call GetCallDetails(int callId);
        void UpdateCallDetails(BO.Call call);
        void DeleteCall(int callId);
        void AddCall(BO.Call call);
        IEnumerable<BO.ClosedCallInList> GetClosedCallsHandledByVolunteer(int volunteerId, BO.Enums.CallType? callTypeFilter = null, BO.Enums.CallFields? sortField = null);
        IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.Enums.CallType? callTypeFilter = null, BO.Enums.CallFields? sortField = null);
        void MarkCallCompletion(int volunteerId, int assignmentId);
        void MarkCallCancellation(int volunteerId, int assignmentId);
        void SelectCallForTreatment(int volunteerId, int callId);
    }
}

