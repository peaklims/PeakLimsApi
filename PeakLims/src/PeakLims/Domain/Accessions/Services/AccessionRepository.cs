namespace PeakLims.Domain.Accessions.Services;

using PeakLims.Domain.Accessions;
using PeakLims.Databases;
using PeakLims.Services;

public interface IAccessionRepository : IGenericRepository<Accession>
{
}

public sealed class AccessionRepository : GenericRepository<Accession>, IAccessionRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public AccessionRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
