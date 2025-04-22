namespace DO;
/// <summary>
/// Enum for the volunteer's role.
/// </summary>
public enum Role
{
    Manager,
    Volunteer
}
/// <summary>
/// Enum for the distance type for the max distance for the volunteer to receive a call.
/// </summary>
public enum DistanceTypes
{
    AerialDistance,
    WalkingDistance,
    DrivingDistance
}
/// <summary>
/// Enum for the call type.
/// </summary>
public enum CallType
{
    Transportation,
    CarAccident,
    VehicleBreakdown,
    SearchAndRescue,
}
/// <summary>
/// Enum for the type of ending of the call.
/// </summary>
public enum EndType
{
    WasTreated,
    SelfCancellation,
    ManagerCancellation,
    Expired
}


