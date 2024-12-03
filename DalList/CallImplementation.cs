namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
/// <summary>
/// The implementation class for the Calls. Implementing all the CRUD functions.
/// </summary>
internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        int newId = Config.NextCallId;
        Call copy = item with { Id = newId};
        DataSource.Calls.Add(copy);
    }

    public void Delete(int id)
    {
        Call? newCall = DataSource.Calls.Find(call => call!.Id == id);
        if (newCall == null)
            throw new Exception($"Call with ID={id} does Not exist");
        else
            DataSource.Calls.Remove(newCall);
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(item => item!.Id == id); //stage 2
    }

    public IEnumerable<Call?> ReadAll(Func<Call?, bool>? filter = null) //stage 2
       => filter == null
           ? DataSource.Calls // החזר את הרשימה כמות שהיא אם אין פילטר
           : DataSource.Calls.Where(filter);

    public void Update(Call item)
    {
        Call? newCall = DataSource.Calls.Find(call => call!.Id == item.Id);
        if (newCall == null)
            throw new Exception($"Call with ID={item.Id} does Not exist");
        else
        {
            DataSource.Calls.Remove(newCall);
            DataSource.Calls.Add(item);
        }
    }
}
