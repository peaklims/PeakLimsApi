namespace PeakLims.Domain.Samples.Services;

using PeakLims.Domain.Samples;
using PeakLims.Databases;
using PeakLims.Services;

public interface ISampleRepository : IGenericRepository<Sample>
{
}

public sealed class SampleRepository : GenericRepository<Sample>, ISampleRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public SampleRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
