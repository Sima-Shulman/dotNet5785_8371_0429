namespace DalApi;
using DO;
internal interface IAssignment
{
    void Create(DO.Iassignment item); //Creates new entity object in DAL
    DO.Iassignment? Read(int id); //Reads entity object by its ID 
    List<DO.Iassignment> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(DO.Iassignment item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
}


