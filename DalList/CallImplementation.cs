namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        int newId = Config.NextCallId;
        Call copy = item with { Id = newId, Call_type = item.Call_type, Verbal_description = item.Verbal_description, Full_address = item.Full_address, Latitude = item.Latitude, Longitude = item.Longitude, Opening_time = item.Opening_time, Max_finish_time = item.Max_finish_time } ;
        DataSource.Calls.Add(copy);
    }

    public void Delete(int id)
    {
        Call? newCall = DataSource.Calls.Find(call => call.Id == id);
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
        Call? newCall = DataSource.Calls.Find(call => call.Id == id);
        return newCall;
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        Call? newCall = DataSource.Calls.Find(call => call.Id == item.Id);
        if (newCall == null)
            throw new Exception($"Call with ID={item.Id} does Not exist");
        else
        {
            DataSource.Calls.Remove(newCall);
            DataSource.Calls.Add(item);
        }
    }
}
