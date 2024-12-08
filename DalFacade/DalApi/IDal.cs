namespace DalApi;

/// <summary>
/// An interface which wraps up all the other interfaces.
/// </summary>
public interface IDal
{
    IAssignment Assignment { get; }
    ICall Call { get; }
    IVolunteer Volunteer { get; }
    IConfig Config { get; }
    void ResetDB();
}
