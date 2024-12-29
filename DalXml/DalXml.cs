using DalApi;
namespace Dal;

sealed internal class DalXml : IDal
{
    private static readonly Lazy<IDal> lazyInstance =
        new Lazy<IDal>(() => new DalXml());
    public static IDal Instance => lazyInstance.Value;
    private DalXml() { }

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}
