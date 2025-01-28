using BlApi;
using Helpers;
using static BO.Enums;

namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    public TimeSpan GetRiskTimeRange()
    {
        return _dal.Config.RiskRange;
    }

    public void InitializeDatabase()
    {
        throw new NotImplementedException();
    }

    public void PromoteClock(BO.Enums.TimeUnit timeUnit)
    {
        switch (timeUnit)
        {
            case BO.Enums.TimeUnit.Minute:
                _dal.Config.Clock
                break;
            case BO.Enums.TimeUnit.Hour:
                break;
            case BO.Enums.TimeUnit.Day:
                break;
            case BO.Enums.TimeUnit.Month:
                break;
            case BO.Enums.TimeUnit.Year:
                break;
            default:
                break;
        }


        ClockManager.UpdateClock(newClock);
    }

    public void ResetDatabase()
    {
        //
        _dal.ResetDB();
    }

    public void SetRiskTimeRange(TimeSpan riskTimeRange)
    {
        _dal.Config.RiskRange = riskTimeRange;
    }
}
