using DalApi;

namespace DO;
/// <summary>
/// Call Entity represents a call with all its props
/// </summary>
/// <param name="id">The call unique id number. A running ID number</param>
/// <param name="call_type">The call's type:</param>
/// <param name="Description">Describe the call in words.</param>
/// <param name="full_address">Full and real address in correct format, of the reading location</param>
/// <param name="latitude">This feature is intended for the purpose of calculating distances between addresses</param>
/// <param name="longitude">This feature is intended for the purpose of calculating distances between addresses</param>
/// <param name="opening_time">Time (date and time) when the call was opened by the manager</param>
/// <param name="max_finish_time">Time (date and time) when the call was opened by the manager</param>
public record Call
(
    CallType CallType,
    string? Description,
    string FullAddress,
    double? Latitude,
    double? Longitude,
    DateTime OpeningTime,
    DateTime? MaxFinishTime
 )
{
    public int Id { get; init; }
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Call() : this(CallType.VehicleBreakdown, null, "", 0, 0, DateTime.Now, null) { }
}
