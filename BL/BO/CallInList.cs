using Helpers;
using static BO.Enums;
namespace BO;
/// <summary>
/// This class represents a call in the system, including its basic details such as ID, type, status, 
/// time-related information, the last volunteer assigned, and the total number of assignments.
/// </summary>
public class CallInList
{
    public int? AssignmentId {  get; set; }
    public int CallId { get; set; }
    public CallType CallType { get; set; }
    public DateTime OpeningTime { get; set; }
    public TimeSpan? TimeLeft { get; set; }
    public string? LastVolunteerName { get; set; }
    public TimeSpan? TotalTime {  get; set; }
    public CallStatus CallStatus { get; set; }
    public int TotalAssignments { get; set; }
    public override string ToString() => this.ToStringProperty();
}