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
        lock (AdminManager.BlMutex)//stage 7
            return AdminManager.Now;
    }
    /// <summary>
    /// Retrieves the current risk time range configured in the system.
    /// </summary>
    /// <returns>A TimeSpan representing the risk time range.</returns>
    public TimeSpan GetRiskTimeRange()
    {
        lock (AdminManager.BlMutex)//stage 7
            return AdminManager.RiskRange;
    }
    /// <summary>
    /// Initializes the database with default test data and resets the system clock.
    /// </summary>
    public void InitializeDatabase()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        lock (AdminManager.BlMutex)//stage 7
            AdminManager.InitializeDB();

    }
    /// <summary>
    /// Promotes the system clock forward by the specified time unit.
    /// </summary>
    /// <param name="timeUnit">The time unit to add (Minute, Hour, Day, etc.).</param>

    public void PromoteClock(BO.Enums.TimeUnit timeUnit)
    {
        lock (AdminManager.BlMutex)//stage 7
        {
            DateTime newClock = timeUnit switch
            {
                TimeUnit.Minute => AdminManager.Now.AddMinutes(1),
                TimeUnit.Hour => AdminManager.Now.AddHours(1),
                TimeUnit.Day => AdminManager.Now.AddDays(1),
                TimeUnit.Month => AdminManager.Now.AddMonths(1),
                TimeUnit.Year => AdminManager.Now.AddYears(1),
                _ => throw new BO.BlInvalidFormatException(nameof(timeUnit) + "Invalid time unit")
            };

            AdminManager.UpdateClock(newClock);
        }
    }
    /// <summary>
    /// Resets the database and updates the system clock.
    /// </summary>
    public void ResetDatabase()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        lock (AdminManager.BlMutex)//stage 7
            AdminManager.ResetDB();
    }
    /// <summary>
    /// Sets the system's risk time range.
    /// </summary>
    /// <param name="riskTimeRange">The new TimeSpan value for the risk time range.</param>
    public void SetRiskTimeRange(TimeSpan riskTimeRange)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        lock (AdminManager.BlMutex)//stage 7
            AdminManager.RiskRange = riskTimeRange;
    }
    #region Stage 5
    public void AddClockObserver(Action clockObserver) {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        AdminManager.ClockUpdatedObservers += clockObserver;
    }
    public void RemoveClockObserver(Action clockObserver) {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ClockUpdatedObservers -= clockObserver;
    }
    public void AddConfigObserver(Action configObserver)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ConfigUpdatedObservers += configObserver;

    }
    public void RemoveConfigObserver(Action configObserver)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        AdminManager.ConfigUpdatedObservers -= configObserver;

    }
    #endregion Stage 5

    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }
    public void StopSimulator()
        => AdminManager.Stop(); //stage 7
}
