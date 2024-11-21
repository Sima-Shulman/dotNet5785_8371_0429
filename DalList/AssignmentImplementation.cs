namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    public int Create(Assignment item)
    {
        int newId = Config.NextAssignmentId;
        Assignment copy = new Assignment(item);
        copy.id = newId;
        DataSource.Assignments.Add(copy);
        return newId;
    }
    
    public void Delete(int id)
    {
        Assignment? newAssignment = DataSource.Assignments.Find(assignment => assignment.id == id);
        if (newAssignment == null)
            throw new NotImplementedException("An object with such an ID does not exist");
        else
            DataSource.Assignments.Remove(newAssignment);
    }    
    
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        Assignment? newAssignment = DataSource.Assignments.Find(assignment => assignment.id == id);
        return newAssignment;
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        Assignment? newAssignment = DataSource.Assignments.Find(assignment => assignment.id == item.id);
        if (newAssignment == null)
        {
            throw new NotImplementedException("An object with such an ID does not exist");
        }
        else
        {
            DataSource.Assignments.Remove(newAssignment);
            DataSource.Assignments.Add(item);

        }
    }
}
