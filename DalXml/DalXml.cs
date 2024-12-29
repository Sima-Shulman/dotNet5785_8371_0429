using DalApi;
namespace Dal;

/// An entity for managing the DB aml files.
/// Inherits and implements the IDal interface by initializing the subinterfaces in the access classes that we implemented in stage 3.
sealed public class DalXml : IDal
{
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public IConfig Config { get; } = new ConfigImplementation();

    /// <summary>
    /// Reset the DB xml files.
    /// </summary>
    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }

}