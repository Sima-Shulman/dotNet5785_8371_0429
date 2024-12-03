using DalApi;
using DO;

namespace Dal;
/// <summary>
/// The implementation class for the Volunteers. Implementing all the CRUD functions.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        Volunteer? existingVolunteer = DataSource.Volunteers.Find(volunteer => volunteer!.Id == item.Id);
        if (existingVolunteer != null)
        {
            throw new Exception($"Volunteer with ID={item.Id} already exist");
        }
        else
        {
            DataSource.Volunteers.Add(item);
        }
    }

    public void Delete(int id)
    {
        Volunteer? newVolunteer = DataSource.Volunteers.Find(volunteer => volunteer!.Id == id);
        if (newVolunteer == null)
            throw new Exception($"Call with ID={id} does Not exist");
        else
            DataSource.Volunteers.Remove(newVolunteer);
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(item => item!.Id == id); //stage 2
    }

    public IEnumerable<Volunteer?> ReadAll(Func<Volunteer?, bool>? filter = null) //stage 2
     => filter == null
         ? DataSource.Volunteers // החזר את הרשימה כמות שהיא אם אין פילטר
         : DataSource.Volunteers.Where(filter);

    public void Update(Volunteer item)
    {
        Volunteer? newVolunteer = DataSource.Volunteers.Find(volunteer => volunteer!.Id == item.Id);
        if (newVolunteer == null)
        {
            throw new Exception($"Volunteer with ID={item.Id} does Not exist");
        }
        else
        {
            DataSource.Volunteers.Remove(newVolunteer);
            DataSource.Volunteers.Add(item);
        }
    }
}
