using Helpers;
using static BO.Enums;
namespace BO;

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
