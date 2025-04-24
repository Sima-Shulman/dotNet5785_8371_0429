namespace BlImplementation;
using BlApi;

/// <summary>
/// Implementation of the Business Logic layer (BL), providing access to 
/// different logical operations via interfaces for volunteers, calls, and admin functionality.
/// </summary>
internal class Bl : IBl
{
    /// <summary>
    /// Business logic operations related to volunteers.
    /// </summary>
    public IVolunteer Volunteer { get; } =  new VolunteerImplementation();
    /// <summary>
    /// Business logic operations related to calls.
    /// </summary>
    public ICall Call { get; } = new CallImplementation();
    /// <summary>
    /// System-level operations and configurations handled by the admin.
    /// </summary>
    public IAdmin Admin { get; } = new AdminImplementation();
}
