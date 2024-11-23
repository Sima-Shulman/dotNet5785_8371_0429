using DalApi;

namespace DO;
/// <summary>
/// Call Entity represents a call with all its props
/// </summary>
/// <param name="id">The call unique id number. A running ID number</param>
/// <param name="call_type">The call's type:</param>
/// <param name="Verbal_description">Describe the call in words.</param>
/// <param name="full_address">Full and real address in correct format, of the reading location</param>
/// <param name="latitude">This feature is intended for the purpose of calculating distances between addresses</param>
/// <param name="longitude">This feature is intended for the purpose of calculating distances between addresses</param>
/// <param name="opening_time">Time (date and time) when the call was opened by the manager</param>
/// <param name="max_finish_time">Time (date and time) when the call was opened by the manager</param>
public record Call

( 
    int Id,
    CallType Call_type,
    string? Verbal_description,
    string Full_address ,
    double Latitude,
    double Longitude ,
    DateTime Opening_time ,
    DateTime Max_finish_time 
 )
{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Call() : this(0, CallType.vehicle_breakdown, null, "", 0, 0, DateTime.Now, DateTime.Now) { }
}
