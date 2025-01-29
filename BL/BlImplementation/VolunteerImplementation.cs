using BlApi;
using Helpers;

namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public BO.Enums.Role EnterSystem(string name, string pass)
    {
        var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v!.FullName == name && VolunteerManager.VerifyPassword(pass,v.Password!));
        return volunteer is null ? throw new BO.BlAuthenticationException("Invalid username or password") : (BO.Enums.Role)volunteer.Role;
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActiveFilter = null, BO.Enums.VolunteerInListFields? fieldSort = null)
    {
        try
        {
            var volunteers = _dal.Volunteer.ReadAll();
            if (isActiveFilter is not null)
                volunteers = volunteers.Where(v => v?.IsActive == isActiveFilter.Value);
            var allVolunteersInList = VolunteerManager.GetVolunteerList(volunteers);
            var sortedVolunteers = fieldSort is not null ? fieldSort switch
            {
                BO.Enums.VolunteerInListFields.FullName => allVolunteersInList.OrderBy(v => v?.FullName).ToList(),
                BO.Enums.VolunteerInListFields.TotalHandledCalls => allVolunteersInList.OrderBy(v => v?.TotalHandledCalls).ToList(),
                BO.Enums.VolunteerInListFields.TotalCanceledCalls => allVolunteersInList.OrderBy(v => v?.TotalCanceledCalls).ToList(),
                BO.Enums.VolunteerInListFields.TotalExpiredCalls => allVolunteersInList.OrderBy(v => v?.TotalExpiredCalls).ToList(),
                BO.Enums.VolunteerInListFields.CallId => allVolunteersInList.OrderBy(v => v?.TotalExpiredCalls).ToList(),
                BO.Enums.VolunteerInListFields.CallType => allVolunteersInList.OrderBy(v => v?.CallType).ToList(),/// למה כתוב שצריך פונקציה נפרדת??
                _ => allVolunteersInList.OrderBy(v => v?.Id).ToList(),
            } : allVolunteersInList.OrderBy(v => v?.Id).ToList();
            return sortedVolunteers;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.GeneralDatabaseException("Error accessing data.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.GeneralDatabaseException("An unexpected error occurred while getting Volunteers.", ex);
        }
    }

    public BO.Volunteer GetVolunteerDetails(int id)
    {
        try
        {
            var doVolunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");
            return Helpers.VolunteerManager.ConvertDoVolunteerToBoVolunteer(doVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.GeneralDatabaseException("Error accessing data.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.GeneralDatabaseException("An unexpected error occurred while getting Volunteers.", ex);
        }
    }

    public void UpdateVolunteerDetails(int requesterId, BO.Volunteer boVolunteer)
    {
        try
        {
            DO.Volunteer? requester = _dal.Volunteer.Read(requesterId) ?? throw new BO.BlDoesNotExistException("Requester does not exist!");
            if (requester.Id != boVolunteer.Id || requester.Role != DO.Role.manager)
                throw new Exception("Requester is not authorized!");
            var existingVolunteer = _dal.Volunteer.Read(boVolunteer.Id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.Id} does not exist");
            Helpers.VolunteerManager.ValidateVolunteer(boVolunteer);
            if (requester.Role != DO.Role.manager && requester.Role != (DO.Role)boVolunteer.Role)
                throw new Exception("Requester is not authorized to change the Role field!");
            var (latitude, longitude) = Helpers.Tools.GetCoordinatesFromAddress(boVolunteer.FullAddress!);
            if (latitude is null || longitude is null)
                throw new BO.GeolocationNotFoundException($"Invalid address: {boVolunteer.FullAddress}");
            boVolunteer.Latitude = latitude;
            boVolunteer.Longitude = longitude;

            DO.Volunteer updatedVolunteer = VolunteerManager.ConvertBoVolunteerToDoVolunteer(boVolunteer);
            _dal.Volunteer.Update(updatedVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.Id} does not exist", ex);
        }
        catch (BO.Exception ex)
        {
        }
    }

    public void DeleteVolunteer(int id)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");
            if (_dal.Assignment.ReadAll(a => a!.VolunteerId == id).Any())
                throw new BO.BlInvalidOperationException($"Cannot delete volunteer with ID={id} as they are handling calls.");
            _dal.Volunteer.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist", ex);
        }
    }

    public void AddVolunteer(BO.Volunteer boVolunteer)
    {
        try
        {
            if (_dal.Volunteer.Read(boVolunteer.Id) is not null)
                throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.Id} already exist");
            Helpers.VolunteerManager.ValidateVolunteer(boVolunteer);
            var (latitude, longitude) = Helpers.Tools.GetCoordinatesFromAddress(boVolunteer.FullAddress!);
            if (latitude is null || longitude is null)
                throw new BO.GeolocationNotFoundException($"Invalid address: {boVolunteer.FullAddress}");
            boVolunteer.Latitude = latitude;
            boVolunteer.Longitude = longitude;

            DO.Volunteer doVolunteer = VolunteerManager.ConvertBoVolunteerToDoVolunteer(boVolunteer);

            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
        catch (BO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
    }
}
