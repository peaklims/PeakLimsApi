namespace PeakLims.Domain.Panels.Services;

using PeakLims.Domain.Panels;
using PeakLims.Databases;
using PeakLims.Services;

public interface IPanelRepository : IGenericRepository<Panel>
{
}

public sealed class PanelRepository : GenericRepository<Panel>, IPanelRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public PanelRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
