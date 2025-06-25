using DalApi;
using DO;
using System.Runtime.CompilerServices;

namespace Dal;
/// <summary>
/// The implementation class for the Volunteers. Implementing all the CRUD functions.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// A function that creates a new object.
    /// </summary>
    /// <param name="item">The new item</param>
    /// <exception cref="DalAlreadyExistsException">An exception in case of attempting to create an item that already exists</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        if (Read(item.Id) is not null)
        {
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exist");
        }
        else
        {
            DataSource.Volunteers.Add(item);
        }
    }
    /// <summary>
    /// A function that deletes a certain item.
    /// </summary>
    /// <param name="id">The item's id</param>
    /// <exception cref="DalDoesNotExistException">An exception in case of attempting to delete an item that does not exist </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Volunteer? newVolunteer = Read(id);
        if (newVolunteer is null)
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist");
        else
            DataSource.Volunteers.Remove(newVolunteer);
    }

    /// <summary>
    /// Delete all the items of this entity.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    /// <summary>
    /// Find an item according to it's id.
    /// </summary>
    /// <param name="id">The item's id</param>
    /// <returns>The item</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(item => item!.Id == id); //stage 2
    }

    /// <summary>
    /// A function that reads all the items of this entity with or without a filtering function.
    /// </summary>
    /// <param name="filter">the filtering function.</param>
    /// <returns>All the items of this entity that match the filter.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer?> ReadAll(Func<Volunteer?, bool>? filter = null) //stage 2
     => filter == null
         ? DataSource.Volunteers // החזר את הרשימה כמות שהיא אם אין פילטר
         : DataSource.Volunteers.Where(filter);

    /// <summary>
    /// A function for updating items.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="DalDoesNotExistException">An exception in case of attempting to update an item that does not exist</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        Volunteer? newVolunteer = Read(item.Id);
        if (newVolunteer is null)
        {
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does Not exist");
        }
        else
        {
            DataSource.Volunteers.Remove(newVolunteer);
            DataSource.Volunteers.Add(item);
        }
    }
    /// <summary>
    /// A function for reading an item with a filter.
    /// </summary>
    /// <param name="filter">The filter function</param>
    /// <returns>The first item of this entity that meets the filter.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
        => DataSource.Volunteers.FirstOrDefault(filter!);
}
