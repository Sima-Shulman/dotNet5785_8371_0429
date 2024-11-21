namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="id"></param>
/// <param name="VolunteerId"></param>
/// <param name="CallId"></param>
/// <param name="start_time"></param>
/// <param name="end_time"></param>
/// <param name="endType"></param>
public record Assignment

(
     int id,
     int VolunteerId,
     int CallId,
     DateTime start_time,
     DateTime? end_time,
     EndType? endType
 )
{
    public Assignment() : this(0, 0, 0, Config.Clock, null, null) { }
}
