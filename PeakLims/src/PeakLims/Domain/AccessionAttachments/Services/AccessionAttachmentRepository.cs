namespace PeakLims.Domain.AccessionAttachments.Services;

using PeakLims.Domain.AccessionAttachments;
using PeakLims.Databases;
using PeakLims.Services;

public interface IAccessionAttachmentRepository : IGenericRepository<AccessionAttachment>
{
}

public sealed class AccessionAttachmentRepository : GenericRepository<AccessionAttachment>, IAccessionAttachmentRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public AccessionAttachmentRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
