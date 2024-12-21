namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

internal class AssignmentImplementation : IAssignment
{
    
    static Assignment GetAssignment(XElement a)
    {
        return new DO.Assignment()
        {
            Id = a.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            VolunteerId = a.ToIntNullable("VolunteerId") ?? throw new FormatException("can't convert VolunteerId"),
            CallId = a.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            Start_time = (DateTime?)a.Element("Start_time") ?? DateTime.Now,
            End_time = (DateTime?)a.Element("End_time"),
            EndType = a.ToEnumNullable<EndType>("EndType") ?? throw new FormatException("can't convert EndType"),
        };
    }

    public void Create(Assignment item)
    {
        XElement assignmentsRoot = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        if (assignmentsRoot.Elements().Any(v => (int?)v.Element("Id") == item.Id))
        {
            throw new InvalidOperationException($"A assignment with ID {item.Id} already exists.");
        }
        XElement newAssignment = new XElement("Assignment",
            new XElement("Id", item.Id),
            new XElement("VolunteerId", item.VolunteerId),
            new XElement("CallId", item.CallId),
            new XElement("Start_time", item.Start_time),
            new XElement("End_time", item.End_time),
            new XElement("EndType", item.EndType)
        );
        assignmentsRoot.Add(newAssignment);
        XMLTools.SaveListToXMLElement(assignmentsRoot, Config.s_assignments_xml);
    }

    public void Delete(int id)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        XElement assignmentToDelete = assignmentsRootElem
            .Elements()
            .FirstOrDefault(v => (int?)v.Element("Id") == id)
            ?? throw new DO.DalDoesNotExistException($"Assignment with ID={id} does not exist");
        assignmentToDelete.Remove();
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }

    public void DeleteAll()
    {
        XElement emptyRoot = new XElement("ArrayOfAssignment");
        XMLTools.SaveListToXMLElement(emptyRoot, Config.s_assignments_xml);
    }

    public Assignment? Read(int id)
    {
        XElement? assignmentElm =
       XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().FirstOrDefault(assign => (int?)assign.Element("Id") == id);
        return assignmentElm is null ? null : GetAssignment(assignmentElm);
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(a => GetAssignment(a)).FirstOrDefault(filter);
    }

    public IEnumerable<Assignment?> ReadAll(Func<Assignment?, bool>? filter = null)
    {
        IEnumerable<Assignment> Assignments = XMLTools
             .LoadListFromXMLElement(Config.s_volunteers_xml)
             .Elements()
             .Select(v => GetAssignment(v));
        return filter == null ? Assignments : Assignments.Where(filter);
    }


    public void Update(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        (assignmentsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist")).Remove();
        assignmentsRootElem.Add(new XElement("Assignment",
            new XElement("Id", item.Id),
            new XElement("CallId", item.CallId),
            new XElement("VolunteerId", item.VolunteerId),
            new XElement("Start_time", item.Start_time),
            new XElement("End_time", item.End_time),
            new XElement("EndType", item.EndType)
        ));
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }
}
