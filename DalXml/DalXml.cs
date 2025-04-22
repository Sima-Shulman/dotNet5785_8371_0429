using DalApi;
namespace Dal;

/// <summary>
/// An entity for managing the DB lists.
/// Inherits and implements the IDal interface by initializing the subinterfaces 
/// in the access classes that we implemented in step 1.
/// </summary>
sealed internal class DalXml : IDal
{
    private static readonly Lazy<IDal> lazyInstance =
        new Lazy<IDal>(() => new DalXml());
    public static IDal Instance => lazyInstance.Value;
    /// <summary>
    /// Empty private ctor.
    /// </summary>
    private DalXml() { }

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
