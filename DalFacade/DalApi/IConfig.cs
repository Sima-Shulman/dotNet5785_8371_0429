namespace DalApi;
/// <summary>
/// Interface for the Config class, with it's functions.
/// </summary>
public interface IConfig
{
    DateTime Clock { get; set; }
    TimeSpan RiskRange { get; set; }
    void Reset();

}
