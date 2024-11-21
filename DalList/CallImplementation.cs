namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public int Create(Call item)
    {
        int newId = Config.NextCallId;
        Call copy = new Call(item);
        copy.Id = newId;
        DataSource.Calls.Add(copy);
        return newId;
    }

    public void Delete(int id)
    {
        Call? newCall = DataSource.Calls.Find(call => call.id == id);
        if (newCall == null)
            throw new NotImplementedException("An object with such an ID does not exist");
        else
            DataSource.Calls.Remove(newCall);
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        Call? newCall = DataSource.Calls.Find(call => call.id == id);
        return newCall;
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        Call? newCall = DataSource.Calls.Find(call => call.id == item.id);
        if (newCall == null)
            throw new NotImplementedException("An object with such an ID does not exist");
        else
        {
            DataSource.Calls.Remove(newCall);
            DataSource.Calls.Add(item);
        }
    }
}
