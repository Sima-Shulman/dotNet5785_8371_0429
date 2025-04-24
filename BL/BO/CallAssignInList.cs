using Helpers;
using static BO.Enums;
namespace BO;
/// <summary>
/// This class represents an assignment of a volunteer to a call, including the volunteer's details, 
/// the start and end times of the assignment, and the type of termination (if applicable).
/// </summary>
public class CallAssignInList
{
    public int? VolunteerId { get; set; }
    public string VolunteerFullName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public EndType? EndType { get; set; }
    public override string ToString() => this.ToStringProperty();
}
