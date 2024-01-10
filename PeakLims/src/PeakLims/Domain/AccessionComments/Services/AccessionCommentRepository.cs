namespace PeakLims.Domain.AccessionComments.Services;

using Microsoft.EntityFrameworkCore;
using PeakLims.Domain.AccessionComments;
using PeakLims.Databases;
using PeakLims.Services;

public interface IAccessionCommentRepository : IGenericRepository<AccessionComment>
{
}

public sealed class AccessionCommentRepository : GenericRepository<AccessionComment>, IAccessionCommentRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public AccessionCommentRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    

    public override async Task<AccessionComment> GetByIdOrDefault(Guid id, bool withTracking = true, CancellationToken cancellationToken = default)
    {
        return withTracking 
            ? await _dbContext.AccessionComments
                .Include(x => x.Accession)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken) 
            : await _dbContext.AccessionComments
                .Include(x => x.Accession)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}
