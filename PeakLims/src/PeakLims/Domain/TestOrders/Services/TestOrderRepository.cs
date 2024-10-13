namespace PeakLims.Domain.TestOrders.Services;

using Microsoft.EntityFrameworkCore;
using Panels;
using PeakLims.Domain.TestOrders;
using PeakLims.Databases;
using PeakLims.Services;

public interface ITestOrderRepository : IGenericRepository<TestOrder>
{
    bool HasPanelAssignedToAccession(Panel panel);
}

public sealed class TestOrderRepository(PeakLimsDbContext dbContext)
    : GenericRepository<TestOrder>(dbContext), ITestOrderRepository
{
    private readonly PeakLimsDbContext _dbContext = dbContext;

    public bool HasPanelAssignedToAccession(Panel panel)
    {
        var tos = _dbContext.TestOrders
            .Include(x => x.Accession)
            .Include(x => x.PanelOrder)
            .ThenInclude(x => x.Panel)
            .Include(x => x.PanelOrder)
            .ThenInclude(x => x.Accession)
            .ToList();
        var response = _dbContext.TestOrders
            .Include(x => x.Accession)
            .Include(x => x.PanelOrder)
            .ThenInclude(x => x.Panel)
            .Include(x => x.PanelOrder)
            .ThenInclude(x => x.Accession)
            .Any(x => x.PanelOrder.Panel == panel);
        return response;
    }
}
