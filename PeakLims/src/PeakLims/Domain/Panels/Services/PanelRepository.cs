namespace PeakLims.Domain.Panels.Services;

using Microsoft.EntityFrameworkCore;
using PeakLims.Domain.Panels;
using PeakLims.Databases;
using PeakLims.Services;

public interface IPanelRepository : IGenericRepository<Panel>
{
    bool Exists(string panelCode, int version);
}

public sealed class PanelRepository : GenericRepository<Panel>, IPanelRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public PanelRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public bool Exists(string panelCode, int version)
    {
        return _dbContext.Panels.Any(x => x.PanelCode == panelCode && x.Version == version);
    }

    public override async Task<Panel> GetByIdOrDefault(Guid id, bool withTracking = true, CancellationToken cancellationToken = default)
    {
        return withTracking 
            ? await _dbContext.Panels
                .Include(x => x.Tests)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken) 
            : await _dbContext.Panels
                .Include(x => x.Tests)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}
