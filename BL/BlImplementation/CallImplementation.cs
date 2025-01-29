using BlApi;
using DO;
namespace BlImplementation;

internal class CallImplementation : ICall
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddCall(BO.Call boCall)
    {
        DO.Call doCall = new((DO.CallType)boCall.CallType, boCall.Verbal_description, boCall.FullAddress,(double?) boCall.Latitude, boCall.Longitude, boCall.Opening_time, boCall.Max_finish_time);
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
            CallType=(BO.Enums)call.CallType,
            Verbal_description=(BO.Enums)call.Verbal_descruption,
            FullAddress=call.FullAddress,
            Latitude=call.Latitude,
            Longitude=call.Longitude,
            Opening_time=call.Opening_time,
            Max_finish_time=call.Max_finish_time,
            CallStatus=??
            AssignmentList=??
        };


    }

    public int[] GetCallQuantitiesByStatus()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.CallInList> GetCallsList(BO.Enums.CallFields? fieldFilter = null, object filterValue = null, BO.Enums.CallFields? sortField = null)
    {
        throw new NotImplementedException();
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
