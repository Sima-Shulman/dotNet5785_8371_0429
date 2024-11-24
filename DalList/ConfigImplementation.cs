using DalApi;

namespace Dal;
/// <summary>
/// The config implamentation class.Implementing all the config's interface's functions.
/// </summary>
public class ConfigImplementation:IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    public void Reset()
    {
        Config.Reset();
    }

}
