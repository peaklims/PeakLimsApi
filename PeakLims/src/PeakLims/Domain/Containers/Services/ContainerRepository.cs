namespace PeakLims.Domain.Containers.Services;

using PeakLims.Domain.Containers;
using PeakLims.Databases;
using PeakLims.Services;

public interface IContainerRepository : IGenericRepository<Container>
{
}

public sealed class ContainerRepository : GenericRepository<Container>, IContainerRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public ContainerRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
