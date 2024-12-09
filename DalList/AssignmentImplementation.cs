namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
/// <summary>
/// The implementation class for the Assignments. Implementing all the CRUD functions.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// A function that creates a new object.
    /// </summary>
    /// <param name="item">The new item</param>
    public void Create(Assignment item)
    {
        int newId = Config.NextAssignmentId;
        Assignment copy = item with { Id = newId };
        DataSource.Assignments.Add(copy);
    }

    /// <summary>
    /// A function that deletes a certain item.
    /// </summary>
    /// <param name="id">The item's id</param>
    /// <exception cref="DalDoesNotExistException">An exception in case of attempting to delete an item that does not exist </exception>
    public void Delete(int id)
    {
        //Assignment? newAssignment = Read(id);
        //if (newAssignment is null)
        //    throw new DalDoesNotExistException($"Assignment with ID={id} does Not exist");
        //else
        //    DataSource.Assignments.Remove(newAssignment);
        throw new DalDeletionImpossible("Can not delete this item.");
    }

    /// <summary>
    /// Delete all the items of this entity.
    /// </summary>
    public void DeleteAll()
    {
        //DataSource.Assignments.Clear();
        throw new DalDeletionImpossible("Can not delete this item.");
    }

    /// <summary>
    /// Find an item according to it's id.
    /// </summary>
    /// <param name="id">The item's id</param>
    /// <returns>The item</returns>
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(item => item!.Id == id); //stage 2
    }

    /// <summary>
    /// A function that reads all the items of this entity with or without a filtering function.
    /// </summary>
    /// <param name="filter">the filtering function.</param>
    /// <returns>All the items of this entity that match the filter.</returns>
    public IEnumerable<Assignment?> ReadAll(Func<Assignment?, bool>? filter = null) //stage 2
        => filter == null
            ? DataSource.Assignments // החזר את הרשימה כמות שהיא אם אין פילטר
            : DataSource.Assignments.Where(filter); // אם יש פילטר, החזר את הרשימה אחרי הפילטר

    /// <summary>
    /// A function for updating items.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="DalDoesNotExistException">An exception in case of attempting to update an item that does not exist</exception>
    public void Update(Assignment item)
    {
        Assignment? newAssignment = Read(item.Id);
        if (newAssignment is null)
        {
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist");
        }
        else
        {
            DataSource.Assignments.Remove(newAssignment);
            DataSource.Assignments.Add(item);

        }
    }
    /// <summary>
    /// A function for reading an item with a filter.
    /// </summary>
    /// <param name="filter">The filter function</param>
    /// <returns>The first item of this entity that meets the filter.</returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    => DataSource.Assignments.FirstOrDefault(filter!);
}
