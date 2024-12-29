namespace Dal;

using DalApi;
/// <summary>
/// The config implementation class.Implementing all the config's interface's functions.
/// </summary>
internal class ConfigImplementation : IConfig
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

    /// <summary>
    /// Reset all the Config fields.
    /// </summary>
    public void Reset()
    {
        Config.Reset();
    }

}
