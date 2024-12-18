namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        List<Volunteer> Courses = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (Courses.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Course with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Courses, Config.s_volunteers_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Volunteer>(), Config.s_volunteers_xml);
    }

    public Volunteer? Read(int id)
    {
        throw new NotImplementedException();
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Volunteer?> ReadAll(Func<Volunteer?, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public void Update(Volunteer item)
    {
        List<Volunteer> Courses = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (Courses.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Course with ID={item.Id} does Not exist");
        Courses.Add(item);
        XMLTools.SaveListToXMLSerializer(Courses, Config.s_volunteers_xml);
    }
}
