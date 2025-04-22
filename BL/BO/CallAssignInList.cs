using Helpers;
using static BO.Enums;
namespace BO;

public class CallAssignInList
{
    public int? VolunteerId { get; set; }
    public string VolunteerFullName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public EndType? EndType { get; set; }
    public override string ToString() => this.ToStringProperty();
}
