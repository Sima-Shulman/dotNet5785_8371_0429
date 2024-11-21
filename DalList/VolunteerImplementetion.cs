using DalApi;
using DO;

namespace Dal;

public class VolunteerImplementetion : IVolunteer
{
    public int Create(Volunteer item)
    {
        Volunteer? existingVolunteer = DataSource.Volunteers.Find(volunteer => volunteer.Id == item.Id);
        if (existingVolunteer != null)
        {
            throw new ArgumentException("An object with this ID already exists");
        }
        else
        {
            DataSource.Volunteers.Add(item);
            return item.Id; //לשאול את סימי
        }
    }

    public void Delete(int id)
    {
        Volunteer? newVolunteer = DataSource.Volunteers.Find(volunteer => volunteer.Id == id);
        if (newVolunteer == null)
            throw new NotImplementedException("An object with such an ID does not exist");
        else
            DataSource.Volunteers.Remove(newVolunteer);
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    public Volunteer? Read(int id)
    {
        Volunteer? newVolunteer = DataSource.Volunteers.Find(volunteer => volunteer.Id == id);
        return newVolunteer;
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {
        Volunteer? newVolunteer = DataSource.Volunteers.Find(volunteer => volunteer.Id == item.Id);
        if (newVolunteer == null)
        {
            throw new NotImplementedException("An object with such an ID does not exist");
        }
        else
        {
            DataSource.Volunteers.Remove(newVolunteer);
            DataSource.Volunteers.Add(item);
        }
    }
}
