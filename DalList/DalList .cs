namespace Dal;
using DalApi;
/// <summary>
/// An entity for managing the DB lists.
/// Inherits and implements the IDal interface by initializing the subinterfaces in the access classes that we implemented in step 1.
/// </summary>
sealed public class DalList : IDal
{
    public ICall Call { get; } = new CallImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public IConfig Config { get; } = new ConfigImplementation();
    /// <summary>
    /// Reset the DB lists.
    /// </summary>
    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Assignment.DeleteAll();
        Call.DeleteAll(); 	  
        Config.Reset();
    }
}

