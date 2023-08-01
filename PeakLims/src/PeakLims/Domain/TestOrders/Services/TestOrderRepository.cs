namespace PeakLims.Domain.TestOrders.Services;

using PeakLims.Domain.TestOrders;
using PeakLims.Databases;
using PeakLims.Services;

public interface ITestOrderRepository : IGenericRepository<TestOrder>
{
}

public sealed class TestOrderRepository : GenericRepository<TestOrder>, ITestOrderRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public TestOrderRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
