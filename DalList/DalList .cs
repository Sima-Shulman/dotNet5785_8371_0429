namespace Dal;
using DalApi;
/// <summary>
/// An entity for managing the DB lists.
/// Inherits and implements the IDal interface by initializing the subinterfaces in the access classes that we implemented in step 1.
/// </summary>
sealed internal class DalList : IDal
{
    private static readonly Lazy<IDal> lazyInstance =
        new Lazy<IDal>(() => new DalList());

    public static IDal Instance => lazyInstance.Value;
    private DalList() { }

    public ICall Call { get; } = new CallImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Assignment.DeleteAll();
        Call.DeleteAll();
        Config.Reset();
    }
}
