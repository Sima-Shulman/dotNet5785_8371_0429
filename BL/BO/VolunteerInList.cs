using Helpers;
using static BO.Enums;
namespace BO;

/// <summary>
/// Represents a simplified view of a volunteer for list display, including status and basic call information.
/// </summary>
public class VolunteerInList
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    public int TotalHandledCalls { get; set; }
    public int TotalCanceledCalls { get; set; }
    public int TotalExpiredCalls { get; set; }
    public int? CallId   { get; set; }
    public CallType CallType { get; set; }
    public override string ToString() => this.ToStringProperty();
}
