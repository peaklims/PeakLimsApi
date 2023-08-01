namespace PeakLims.Domain.RolePermissions.Services;

using PeakLims.Domain.RolePermissions;
using PeakLims.Databases;
using PeakLims.Services;

public interface IRolePermissionRepository : IGenericRepository<RolePermission>
{
}

public sealed class RolePermissionRepository : GenericRepository<RolePermission>, IRolePermissionRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public RolePermissionRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
