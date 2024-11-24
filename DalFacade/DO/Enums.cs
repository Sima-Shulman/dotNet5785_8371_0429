namespace DO;
/// <summary>
/// Enum for the volunteer's role.
/// </summary>
public enum Role
{
    manager,
    Volunteer
}
/// <summary>
/// Enum for the distance type for the max distance for the volunteer to receive a call.
/// </summary>
public enum DistanceTypes
{
    aerial_distance,
    walking_distance,
    driving_distance
}
/// <summary>
/// Enum for the call type.
/// </summary>
public enum CallType
{
    transportation,
    car_accident,
    vehicle_breakdown,
    search_and_rescue,
}
/// <summary>
/// Enum for the type of endung of the call.
/// </summary>
public enum EndType
{
    was_treated,
    self_cancellation,
    manager_cancellation,
    expired
}


