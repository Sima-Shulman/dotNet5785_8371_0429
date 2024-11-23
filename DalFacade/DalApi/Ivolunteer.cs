namespace DalApi;
using DO;
/// <summary>
/// 
/// </summary>
public interface IVolunteer
{
    void Create(DO.Volunteer item); //Creates new entity object in DAL
    DO.Volunteer? Read(int id); //Reads entity object by its ID
    List<DO.Volunteer> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(DO.Volunteer item); //Updates entity object
    void Delete(int id); //Deletes an object by is Id
    void DeleteAll(); //Delete all entity objects
}
