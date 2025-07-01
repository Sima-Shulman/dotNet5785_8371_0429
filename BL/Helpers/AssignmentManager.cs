using DalApi;
using System;

namespace Helpers;
/// <summary>
/// Provides static access to assignment-related operations using the data layer.
/// </summary>
internal static class AssignmentManager
{
    private static IDal s_dal = Factory.Get;
    //internal static ObserverManager Observers = new(); //stage 5 

    /// <summary>
    /// Periodically updates assignments that should be marked as expired based on a time window.
    /// </summary>
    /// <param name="oldClock">The previous timestamp before update.</param>
    /// <param name="newClock">The current timestamp after update.</param>
    internal static void PeriodicAssignmentUpdates(DateTime oldClock, DateTime newClock)
    {
        List<DO.Call?> expiredCalls = s_dal.Call.ReadAll(c => c?.MaxFinishTime > newClock).ToList();

        expiredCalls.ForEach(call =>
        {
            //List<DO.Assignment?> assignments = s_dal.Assignment.ReadAll(a => a?.CallId == call?.Id).ToList();
            List<DO.Assignment?> assignmentsNotTreated = s_dal.Assignment.ReadAll(a => a?.CallId == call?.Id && (a?.EndType != DO.EndType.WasTreated && a?.EndType != DO.EndType.Expired)).ToList();
            if (assignmentsNotTreated.Any())//there is an assignment to the call that is not yet finished
            {
                assignmentsNotTreated.ForEach((assignment) =>
                {
                    s_dal.Assignment.Update(assignment! with
                    {
                        EndTime = AdminManager.Now,
                        EndType = (DO.EndType)BO.Enums.EndType.Expired
                    });
                    VolunteerManager.Observers.NotifyItemUpdated(assignment.VolunteerId);
                });
                CallManager.Observers.NotifyItemUpdated(call!.Id);
            }
        });
        CallManager.Observers.NotifyListUpdated();
        VolunteerManager.Observers.NotifyListUpdated();

    }

}
