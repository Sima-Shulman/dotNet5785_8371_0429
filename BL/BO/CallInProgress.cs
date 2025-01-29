using Helpers;
using static BO.Enums;
namespace BO;

public class CallInProgress
{
    public int Id { get; set; }
    public int CallId { get; set; }
    public CallType CallType { get; set; }
    public string? Verbal_description { get; set; }
    public string FullAddress { get; set; }
    public DateTime Opening_time { get; set; }
    public DateTime Max_finish_time { get; set; }
    public DateTime  Start_time { get; set; }
    public double CallDistance { get; set; }
    public CallStatus CallStatus { get; set; }
    public override string ToString() => this.ToStringProperty();
}