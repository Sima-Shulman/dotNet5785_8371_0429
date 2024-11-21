using DalApi;

namespace DO;

public record Call

( 
    int id,
    CallType call_type,
    int? Verbal_description,
    string full_address ,
    double latitude,
    double longitude ,
    DateTime opening_time ,
    DateTime max_finish_time 
 )
{
    public Call() : this(0, , null , "",0 , 0 ,DateTime.Now,DateTime.Now) {}
}
