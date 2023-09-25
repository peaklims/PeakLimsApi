namespace PeakLims.Domain.Accessions.Services;

using Microsoft.EntityFrameworkCore;
using PeakLims.Domain.Accessions;
using PeakLims.Databases;
using PeakLims.Services;

public interface IAccessionRepository : IGenericRepository<Accession>
{
    public Task<Accession> GetAccessionForStatusChange(Guid id, CancellationToken cancellationToken = default);
    public Task<Accession> GetWithTestOrderWithChildren(Guid accessionId, bool withTracking, CancellationToken cancellationToken = default);
}

public sealed class AccessionRepository : GenericRepository<Accession>, IAccessionRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public AccessionRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Accession> GetAccessionForStatusChange(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Accessions
            .Include(x => x.TestOrders)
            .ThenInclude(x => x.Test)
            .Include(x => x.AccessionContacts)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Accession> GetWithTestOrderWithChildren(Guid accessionId, bool withTracking, CancellationToken cancellationToken = default)
    {
        return withTracking
            ? _dbContext.Accessions
                .Include(x => x.TestOrders)
                .ThenInclude(x => x.Test)
                .Include(x => x.TestOrders)
                .ThenInclude(x => x.AssociatedPanel)
                .FirstOrDefaultAsync(x => x.Id == accessionId, cancellationToken)
            : _dbContext.Accessions
                .Include(x => x.TestOrders)
                .ThenInclude(x => x.Test)
                .Include(x => x.TestOrders)
                .ThenInclude(x => x.AssociatedPanel)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == accessionId, cancellationToken);
    }
}
