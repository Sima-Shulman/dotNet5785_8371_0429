using BlApi;
using Helpers;

namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddVolunteer(BO.Volunteer boVolunteer)
    {
        DO.Volunteer doVolunteer = new(boVolunteer.Id, boVolunteer.FullName, boVolunteer.CellphoneNumber, boVolunteer.Email, boVolunteer.FullAddress, boVolunteer.Latitude, boVolunteer.Longitude, (DO.Role)boVolunteer.Role, boVolunteer.IsActive, (DO.DistanceTypes)boVolunteer.DistanceTypes, boVolunteer.MaxDistance, boVolunteer.Password);
        try
        {
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
    }

    public void DeleteVolunteer(int id)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");
            if (_dal.Assignment.ReadAll().Any(a => a!.VolunteerId == id))
                throw new BO.BlInvalidOperationException($"Cannot delete volunteer with ID={id} as they are handling calls.");
            _dal.Volunteer.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist", ex);
        }
    }

    public BO.Enums.Role EnterSystem(string name, string pass)
    {
        var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v!.FullName == name && v.Password == pass);

        if (volunteer == null)
            throw new BO.BlAuthenticationException("Invalid username or password");

        return (BO.Enums.Role)volunteer.Role;
    }

    public BO.Volunteer GetVolunteerDetails(int id)
    {
        var doVolunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");
        var handledCalls = _dal.Call.ReadAll()?.Count(c => c.EndType == DO.Enums.EndType.was_treated) ?? 0;
        var canceledCalls = _dal.Call.ReadAll()?.Count(c => c.CallStatus == DO.Enums.CallStatus.Canceled) ?? 0;
        var expiredCalls = _dal.Call.ReadAll()?.Count(c => c.CallStatus == DO.Enums.CallStatus.Expired) ?? 0;
        return new()
        {
            Id = id,
            FullName = doVolunteer.FullName,
            CellphoneNumber=doVolunteer.CellphoneNumber,
            Email = doVolunteer.Email,
            Password = doVolunteer.Password,
            FullAddress = doVolunteer.FullAddress,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            Role = doVolunteer.Role,
            IsActive = doVolunteer.IsActive,
            DistanceTypes = doVolunteer.DistanceTypes,
            MaxDistance=doVolunteer.MaxDistance,

        };
    }

    public IEnumerable<BO.VolunteerInList> ReadAll(bool? isActive = null, BO.Enums.VolunteerFields? fieldFilter = null)
    {
        var volunteers = _dal.Volunteer.ReadAll();

        if (isActive.HasValue)
            volunteers = volunteers.Where(v => v?.IsActive == isActive.Value);

        var sortedVolunteers = fieldFilter switch
        {
            BO.Enums.VolunteerFields.FullName => volunteers.OrderBy(v => v?.FullName),
            BO.Enums.VolunteerFields.Email => volunteers.OrderBy(v => v?.Email),
            BO.Enums.VolunteerFields.Role => volunteers.OrderBy(v => v?.Role),
            _ => volunteers.OrderBy(v => v?.Id),
        };

        return sortedVolunteers.Select(v => new BO.VolunteerInList
        {
            Id = v.Id,
            FullName = v.FullName,
            Email = v.Email,
            Role = (BO.Enums.Role)v.Role,
            IsActive = v.IsActive,
        });
    }

    public void UpdateVolunteerDetails(int id, BO.Volunteer boVolunteer)
    {
        if (!ValidationHelper.IsValidEmail(boVolunteer.Email))
            throw new BO.BlInvalidInputException("Invalid email format");

        if (!ValidationHelper.IsValidId(boVolunteer.Id))
            throw new BO.BlInvalidInputException("Invalid ID format");

        var existingVolunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");

        DO.Volunteer updatedVolunteer = new(
            boVolunteer.Id,
            boVolunteer.FullName,
            boVolunteer.CellphoneNumber,
            boVolunteer.Email,
            boVolunteer.FullAddress,
            boVolunteer.Latitude,
            boVolunteer.Longitude,
            (DO.Role)boVolunteer.Role,
            boVolunteer.IsActive,
            (DO.DistanceTypes)boVolunteer.DistanceTypes,
            boVolunteer.MaxDistance,
            boVolunteer.Password
        );

        try
        {
            _dal.Volunteer.Update(updatedVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist", ex);
        }
    }



}
