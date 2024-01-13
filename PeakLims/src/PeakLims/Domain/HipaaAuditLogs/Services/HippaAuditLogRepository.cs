namespace PeakLims.Domain.HipaaAuditLogs.Services;

using PeakLims.Domain.HipaaAuditLogs;
using PeakLims.Databases;
using PeakLims.Services;

public interface IHipaaAuditLogRepository : IGenericRepository<HipaaAuditLog>
{
}

public sealed class HipaaAuditLogRepository : GenericRepository<HipaaAuditLog>, IHipaaAuditLogRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public HipaaAuditLogRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
