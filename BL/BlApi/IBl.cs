namespace BlApi;
public interface IBl
{
    /// <summary>
    /// Interface for the Business Logic layer providing access to various entities like Volunteer, Call, and Admin.
    /// </summary>
    IVolunteer Volunteer { get; }
    ICall Call { get; }
    IAdmin Admin { get; }
}

