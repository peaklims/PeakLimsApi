namespace PeakLims.Domain.Tests.Services;

using Microsoft.EntityFrameworkCore;
using PeakLims.Domain.Tests;
using PeakLims.Databases;
using PeakLims.Services;

public interface ITestRepository : IGenericRepository<Test>
{
}

public sealed class TestRepository(PeakLimsDbContext dbContext) : GenericRepository<Test>(dbContext), ITestRepository
{
    private readonly PeakLimsDbContext _dbContext = dbContext;

    public override async Task<Test> GetByIdOrDefault(Guid id, bool withTracking = true, CancellationToken cancellationToken = default)
    {
        return withTracking 
            ? await _dbContext.Tests
                .Include(x => x.PanelTestAssignments)
                .ThenInclude(x => x.Panel)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken) 
            : await _dbContext.Tests
                .Include(x => x.PanelTestAssignments)
                .ThenInclude(x => x.Panel)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}
