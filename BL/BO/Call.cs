using Helpers;
using static BO.Enums;
namespace BO;

public class Call
{
    public int Id { get; set; }
    public CallType CallType { get; set; }
    public string? Description { get; set; }
    public string FullAddress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime OpeningTime { get; set; }
    public DateTime? MaxFinishTime { get; set; }
    public CallStatus CallStatus { get; set; }
    public List<BO.CallAssignInList>? AssignmentsList { get; set; }
    public override string ToString() => this.ToStringProperty();


}
