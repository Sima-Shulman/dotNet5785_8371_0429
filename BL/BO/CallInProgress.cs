using Helpers;
using static BO.Enums;
namespace BO;
/// <summary>
/// This class represents a call that is in progress in the system, including its basic details such as ID, type, status, 
/// description, address, time-related information, and the distance of the call.
/// </summary>
public class CallInProgress
{
    public int Id { get; set; }
    public int CallId { get; set; }
    public CallType CallType { get; set; }
    public string? Description { get; set; }
    public string FullAddress { get; set; }
    public DateTime OpeningTime { get; set; }
    public DateTime? MaxFinishTime { get; set; }
    public DateTime  StartTime { get; set; }
    public double CallDistance { get; set; }
    public CallStatus CallStatus { get; set; }
    public override string ToString() => this.ToStringProperty();
}