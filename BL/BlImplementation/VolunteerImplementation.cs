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
        _dal.Delete(id);
    }

    public BO.Enums.Role EnterSystem(string name, string pass)
    {
        throw new NotImplementedException();
    }

    public BO.Volunteer GetVolunteerDetails(int id)
    {
        var doVolunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");
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
        throw new NotImplementedException();
    }

    public void UpdateVolunteerDetails(int id, BO.Volunteer boVolunteer)
    {
        throw new NotImplementedException();
    }
}
