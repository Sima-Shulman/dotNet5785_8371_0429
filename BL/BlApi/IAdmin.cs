
namespace BlApi
{
    /// <summary>
    /// Interface for admin functionalities such as time control and database management.
    /// </summary>
    public interface IAdmin
    {
        DateTime GetClock();
        void PromoteClock(BO.Enums.TimeUnit timeUnit);
        TimeSpan GetRiskTimeRange();
        void SetRiskTimeRange(TimeSpan riskTimeRange);
        void ResetDatabase();
        void InitializeDatabase();
    }

}
