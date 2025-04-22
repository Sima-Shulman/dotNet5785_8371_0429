namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// The implementation class for the Assignments. Implementing all the CRUD functions.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Get a copy of an Assignment from the XML file.
    /// </summary>
    /// <param name="a">The assignment you want</param>
    /// <returns>A copy of the a assignment</returns>
    /// <exception cref="FormatException"></exception>
    static Assignment GetAssignment(XElement a)
    {
        return new DO.Assignment()
        {
            Id = a.ToIntNullable("Id") ?? throw new FormatException("Can't convert Id"),
            CallId = a.ToIntNullable("CallId") ?? throw new FormatException("Can't convert CallId"),
            VolunteerId = a.ToIntNullable("VolunteerId") ?? throw new FormatException("Can't convert VolunteerId"),
            StartTime = a.ToDateTimeNullable("Start_time") ?? throw new FormatException("Can't convert Start_time"),
            EndTime = a.ToDateTimeNullable("End_time") /*?? throw new FormatException("Can't convert End_time")*/,
            EndType = a.ToEnumNullable<EndType>("EndType")/* ?? throw new FormatException("Can't convert EndType")*/

        };
    }

    /// <summary>
    /// A function that creates a new object.
    /// </summary>
    /// <param name="item">The new item</param>
    public void Create(Assignment item)
    {
        XElement assignmentsRoot = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        if (assignmentsRoot.Elements().Any(v => (int?)v.Element("Id") == item.Id))
        {
            throw new InvalidOperationException($"A assignment with ID {item.Id} already exists.");
        }
        XElement newAssignment = new XElement("Assignment",
            new XElement("Id", Config.NextAssignmentId),
            new XElement("VolunteerId", item.VolunteerId),
            new XElement("CallId", item.CallId),
            new XElement("Start_time", item.StartTime),
            new XElement("End_time", item.EndTime),
            new XElement("EndType", item.EndType)
        );
        assignmentsRoot.Add(newAssignment);
        XMLTools.SaveListToXMLElement(assignmentsRoot, Config.s_assignments_xml);
    }

    /// <summary>
    /// A function that deletes a certain item.
    /// </summary>
    /// <param name="id">The item's id</param>
    /// <exception cref="DalDoesNotExistException">An exception in case of attempting to delete an item that does not exist </exception>
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

    /// <summary>
    /// Delete all the items of this entity.
    /// </summary>
    public void DeleteAll()
    {
        XElement emptyRoot = new XElement("ArrayOfAssignment");
        XMLTools.SaveListToXMLElement(emptyRoot, Config.s_assignments_xml);
    }

    /// <summary>
    /// Find an item according to it's id.
    /// </summary>
    /// <param name="id">The item's id</param>
    /// <returns>The item</returns>
    public Assignment? Read(int id)
    {
        XElement? assignmentElm =
       XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().FirstOrDefault(assign => (int?)assign.Element("Id") == id);
        return assignmentElm is null ? null : GetAssignment(assignmentElm);
    }

    /// <summary>
    /// A function for reading an item with a filter.
    /// </summary>
    /// <param name="filter">The filter function</param>
    /// <returns>The first item of this entity that meets the filter.</returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(a => GetAssignment(a)).FirstOrDefault(filter);
    }

    /// <summary>
    /// A function that reads all the items of this entity with or without a filtering function.
    /// </summary>
    /// <param name="filter">the filtering function.</param>
    /// <returns>All the items of this entity that match the filter.</returns>
    public IEnumerable<Assignment?> ReadAll(Func<Assignment?, bool>? filter = null)
    {
        IEnumerable<Assignment> Assignments = XMLTools
             .LoadListFromXMLElement(Config.s_assignments_xml)
             .Elements()
             .Select(v => GetAssignment(v));
        return filter == null ? Assignments : Assignments.Where(filter);
    }

    /// <summary>
    /// A function for updating items.
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="DalDoesNotExistException">An exception in case of attempting to update an item that does not exist</exception>
    public void Update(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        (assignmentsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist")).Remove();
        assignmentsRootElem.Add(new XElement("Assignment",
            new XElement("Id", item.Id),
            new XElement("CallId", item.CallId),
            new XElement("VolunteerId", item.VolunteerId),
            new XElement("Start_time", item.StartTime),
            new XElement("End_time", item.EndTime),
            new XElement("EndType", item.EndType)
        ));
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }
}
