namespace PeakLims.Domain.AccessionComments.Services;

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
}
