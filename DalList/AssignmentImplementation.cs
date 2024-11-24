namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
/// <summary>
/// The implementation class for the Assignments. Implementing all the CRUD functions.
/// </summary>
public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int newId = Config.NextAssignmentId;
        Assignment copy = item with { Id = newId};
        DataSource.Assignments.Add(copy);
    }
    
    public void Delete(int id)
    {
        Assignment? newAssignment = DataSource.Assignments.Find(assignment => assignment!.Id == id);
        if (newAssignment == null)
            throw new Exception($"Assignment with ID={id} does Not exist");
        else
            DataSource.Assignments.Remove(newAssignment);
    }    
    
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        Assignment? newAssignment = DataSource.Assignments.Find(assignment => assignment!.Id == id);
        return newAssignment;
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments!);
    }

    public void Update(Assignment item)
    {
        Assignment? newAssignment = DataSource.Assignments.Find(assignment => assignment!.Id == item.Id);
        if (newAssignment == null)
        {
            throw new Exception($"Assignment with ID={item.Id} does Not exist");
        }
        else
        {
            DataSource.Assignments.Remove(newAssignment);
            DataSource.Assignments.Add(item);

        }
    }
}
