using Helpers;
using static BO.Enums;
namespace BO;

public class ClosedCallInList
{
    public int Id   { get; set; }
    public CallType CallType { get; set; }
    public string FullAddress { get; set; }
    public DateTime Opening_time { get; set; }
    public DateTime Start_time { get; set; }
    public DateTime? End_time { get; set; }
    public EndType? EndType { get; set; }
    public override string ToString() => this.ToStringProperty();
}
