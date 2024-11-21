using DalApi;

namespace DO;

public record Call
{
    public CallType call_type { get; set; }
    public int? Verbal_description { get; set; }
    public string full_adress { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public DateTime opening_time { get; set; }
    public DateTime max_finish_time { get; set; }


}
