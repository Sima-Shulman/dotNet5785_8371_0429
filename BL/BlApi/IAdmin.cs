
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
        #region Stage 5
        void AddConfigObserver(Action configObserver);
        void RemoveConfigObserver(Action configObserver);
        void AddClockObserver(Action clockObserver);
        void RemoveClockObserver(Action clockObserver);
        #endregion Stage 5
    }

}
