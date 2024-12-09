using DO;

namespace DalApi;
/// <summary>
///A generic interface that all entity interfaces inherit from. 
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public interface ICrud<T> where T : class
{
    void Create(T item); //Creates new entity object in DAL
    T? Read(int id); //Reads entity object by its ID 
    IEnumerable<T?> ReadAll(Func<T?, bool>? filter = null);//Read all items with a filtering function or without.
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
    void Update(T item);//Update an item.
    T? Read(Func<T, bool> filter); // Read with a filtering function.

}
