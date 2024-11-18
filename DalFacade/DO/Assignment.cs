namespace DO;

public record Assignment
{
    public int VolunteerId { get; set; }
    public DateTime start_time { get; set; }
    public DateTime? end_time { get; set; }
   public  EndType? endType { get; set; }

}
