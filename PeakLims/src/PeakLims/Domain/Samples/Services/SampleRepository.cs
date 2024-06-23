namespace PeakLims.Domain.Samples.Services;

using Microsoft.EntityFrameworkCore;
using PeakLims.Domain.Samples;
using PeakLims.Databases;
using PeakLims.Services;

public interface ISampleRepository : IGenericRepository<Sample>
{
}

public sealed class SampleRepository(PeakLimsDbContext dbContext)
    : GenericRepository<Sample>(dbContext), ISampleRepository
{
    private readonly PeakLimsDbContext _dbContext = dbContext;

    public override async Task<Sample> GetByIdOrDefault(Guid id, bool withTracking = true, CancellationToken cancellationToken = default)
    {
        return withTracking 
            ? await _dbContext.Samples
                .Include(x => x.Container)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken) 
            : await _dbContext.Samples
                .Include(x => x.Container)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}
