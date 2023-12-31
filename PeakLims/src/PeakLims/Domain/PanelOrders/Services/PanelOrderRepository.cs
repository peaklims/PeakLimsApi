namespace PeakLims.Domain.PanelOrders.Services;

using PeakLims.Domain.PanelOrders;
using PeakLims.Databases;
using PeakLims.Services;

public interface IPanelOrderRepository : IGenericRepository<PanelOrder>
{
}

public sealed class PanelOrderRepository : GenericRepository<PanelOrder>, IPanelOrderRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public PanelOrderRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
