using Helpers;
using static BO.Enums;
namespace BO;
/// <summary>
/// This class represents a closed call in the system, including details such as the call's ID, type, address, time-related
/// information, and the type of end event, if applicable.
/// </summary>
public class ClosedCallInList
{
    public int Id   { get; set; }
    public CallType CallType { get; set; }
    public string FullAddress { get; set; }
    public DateTime OpeningTime { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public EndType? EndType { get; set; }
    public override string ToString() => this.ToStringProperty();
}
