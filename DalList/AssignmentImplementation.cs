namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
/// <summary>
/// The implementation class for the Assignments. Implementing all the CRUD functions.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int newId = Config.NextAssignmentId;
        Assignment copy = item with { Id = newId };
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
        return DataSource.Assignments.FirstOrDefault(item => item!.Id == id); //stage 2
    }
    public IEnumerable<Assignment?> ReadAll(Func<Assignment?, bool>? filter = null) //stage 2
        => filter == null
            ? DataSource.Assignments // החזר את הרשימה כמות שהיא אם אין פילטר
            : DataSource.Assignments.Where(filter); // אם יש פילטר, החזר את הרשימה אחרי הפילטר

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
