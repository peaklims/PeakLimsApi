namespace PeakLims.Services;

using PeakLims.Databases;

public interface IUnitOfWork : IPeakLimsScopedService
{
    Task<int> CommitChanges(CancellationToken cancellationToken = default);
}

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly PeakLimsDbContext _dbContext;

    public UnitOfWork(PeakLimsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CommitChanges(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
