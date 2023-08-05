namespace PeakLims.Domain.TestOrders.Services;

using Microsoft.EntityFrameworkCore;
using Panels;
using PeakLims.Domain.TestOrders;
using PeakLims.Databases;
using PeakLims.Services;

public interface ITestOrderRepository : IGenericRepository<TestOrder>
{
    void CleanupOrphanedTestOrders();
    bool HasPanelAssignedToAccession(Panel panel);
}

public sealed class TestOrderRepository : GenericRepository<TestOrder>, ITestOrderRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public TestOrderRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void CleanupOrphanedTestOrders()
    {
        var testOrders = _dbContext.TestOrders.Where(x => x.Accession == null).ToList();
        _dbContext.TestOrders.RemoveRange(testOrders);
    }

    public bool HasPanelAssignedToAccession(Panel panel)
    {
        return _dbContext.TestOrders
            .Include(x => x.Accession)
            .Include(x => x.AssociatedPanel)
            .Any(x => x.Accession != null && x.AssociatedPanel == panel);
    }
}
