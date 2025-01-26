using BlApi;
using BO;

namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public DateTime GetClock()
    {
        throw new NotImplementedException();
    }

    public TimeSpan GetRiskTimeRange()
    {
        throw new NotImplementedException();
    }

    public void InitializeDatabase()
    {
        throw new NotImplementedException();
    }

    public void PromoteClock(Enums.TimeUnit timeUnit)
    {
        throw new NotImplementedException();
    }

    public void ResetDatabase()
    {
        throw new NotImplementedException();
    }

    public void SetRiskTimeRange(TimeSpan riskTimeRange)
    {
        throw new NotImplementedException();
    }
}
