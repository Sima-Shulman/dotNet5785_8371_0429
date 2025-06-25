namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// The implementation class for the Calls. Implementing all the CRUD functions.
/// </summary>
internal class CallImplementation : ICall
{
    /// <summary>
    /// A function that creates a new object.
    /// </summary>
    /// <param name="item">The new item</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        int newId = Config.NextCallId;
        Call copy = item with { Id = newId};
        DataSource.Calls.Add(copy);
    }

    /// <summary>
    /// A function that deletes a certain item.
    /// </summary>
    /// <param name="id">The item's id</param>
    /// <exception cref="DalDoesNotExistException">An exception in case of attempting to delete an item that does not exist </exception>
     [MethodImpl(MethodImplOptions.Synchronized)] 
    public void Delete(int id)
    {
        Call? newCall = Read(id);
        if (newCall is null)
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist");
        else
            DataSource.Calls.Remove(newCall);
    }

    /// <summary>
    /// Delete all the items of this entity.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    /// <summary>
    /// Find an item according to it's id.
    /// </summary>
    /// <param name="id">The item's id</param>
    /// <returns>The item</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(item => item!.Id == id); //stage 2
    }

    /// <summary>
    /// A function that reads all the items of this entity with or without a filtering function.
    /// </summary>
    /// <param name="filter">the filtering function.</param>
    /// <returns>All the items of this entity that match the filter.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call?> ReadAll(Func<Call?, bool>? filter = null) //stage 2
       => filter == null
           ? DataSource.Calls // החזר את הרשימה כמות שהיא אם אין פילטר
           : DataSource.Calls.Where(filter);

    /// <summary>
    /// A function for updating items.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="DalDoesNotExistException">An exception in case of attempting to update an item that does not exist</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        Call? newCall = Read(item.Id);
        if (newCall is null)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does Not exist");
        else
        {
            DataSource.Calls.Remove(newCall);
            DataSource.Calls.Add(item);
        }
    }
    /// <summary>
    /// A function for reading an item with a filter.
    /// </summary>
    /// <param name="filter">The filter function</param>
    /// <returns>The first item of this entity that meets the filter.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    => DataSource.Calls.FirstOrDefault(filter!);
}
