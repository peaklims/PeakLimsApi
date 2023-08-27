namespace PeakLims.Domain.AccessionContacts.Services;

using PeakLims.Domain.AccessionContacts;
using PeakLims.Databases;
using PeakLims.Services;

public interface IAccessionContactRepository : IGenericRepository<AccessionContact>
{
}

public sealed class AccessionContactRepository : GenericRepository<AccessionContact>, IAccessionContactRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public AccessionContactRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
