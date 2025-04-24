using BlApi;
using Helpers;
using static BO.Enums;

namespace BlImplementation;
/// <summary>
/// The implementation class for the Admin interface, providing system-level functionalities
/// such as clock control and database management.
/// </summary>
internal class AdminImplementation : IAdmin
{
   
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    /// <summary>
    /// Returns the current system clock.
    /// </summary>
    /// <returns>The current DateTime of the system.</returns>
    public DateTime GetClock()
    {
        return ClockManager.Now;
    }
    /// <summary>
    /// Retrieves the current risk time range configured in the system.
    /// </summary>
    /// <returns>A TimeSpan representing the risk time range.</returns>
    public TimeSpan GetRiskTimeRange()
    {
        return _dal.Config.RiskRange;
    }
    /// <summary>
    /// Initializes the database with default test data and resets the system clock.
    /// </summary>
    public void InitializeDatabase()
    {
        _dal.ResetDB();
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }
    /// <summary>
    /// Promotes the system clock forward by the specified time unit.
    /// </summary>
    /// <param name="timeUnit">The time unit to add (Minute, Hour, Day, etc.).</param>

    public void PromoteClock(BO.Enums.TimeUnit timeUnit)
    {
        DateTime newClock = timeUnit switch
        {
            TimeUnit.Minute => ClockManager.Now.AddMinutes(1),
            TimeUnit.Hour => ClockManager.Now.AddHours(1),
            TimeUnit.Day => ClockManager.Now.AddDays(1),
            TimeUnit.Month => ClockManager.Now.AddMonths(1),
            TimeUnit.Year => ClockManager.Now.AddYears(1),
            _ => throw new BO.BlInvalidFormatException(nameof(timeUnit) + "Invalid time unit")
        };

        ClockManager.UpdateClock(newClock);
    }
    /// <summary>
    /// Resets the database and updates the system clock.
    /// </summary>
    public void ResetDatabase()
    {
        _dal.Config.Reset();
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);
    }
    /// <summary>
    /// Sets the system's risk time range.
    /// </summary>
    /// <param name="riskTimeRange">The new TimeSpan value for the risk time range.</param>
    public void SetRiskTimeRange(TimeSpan riskTimeRange)
    {
        _dal.Config.RiskRange = riskTimeRange;
    }
}
