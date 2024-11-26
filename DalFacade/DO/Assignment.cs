namespace DO;

/// <summary>
/// Assignment Entity represents an assignment with all its props
/// </summary>
/// <param name="id"> Represents a number that uniquely identifies the allocation entity. </param>
/// <param name="VolunteerId">Represents the ID of the volunteer who chose to take care of the call..</param>
/// <param name="CallId">Represents a number that identifies the call that the volunteer chose to handle. ID number runs.</param>
/// <param name="start_time">The time when for the first time the current volunteer chose to take care of it.</param>
/// <param name="end_time">Time (date and time) when the current volunteer finished handling the current call.</param>
/// <param name="endType">The way in which the treatment of the current call ended by the current volunteer:
/// handled, self cancel, manager cancel, manager cancel.</param>
public record Assignment

(
     int VolunteerId,
     int CallId,
     DateTime Start_time,
     DateTime? End_time,
     EndType? EndType
 )
{
    public int Id { get; init; }
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Assignment() : this(0, 0, DateTime.Now, null, null) { }
}
