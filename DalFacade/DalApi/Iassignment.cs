namespace DalApi;
using DO;
public interface IAssignment
{
    void Create(DO.Assignment item); //Creates new entity object in DAL
    DO.Assignment? Read(int id); //Reads entity object by its ID 
    List<DO.Assignment> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(DO.Assignment item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
}


