namespace PeakLims.Domain.HealthcareOrganizations.Services;

using PeakLims.Domain.HealthcareOrganizations;
using PeakLims.Databases;
using PeakLims.Services;

public interface IHealthcareOrganizationRepository : IGenericRepository<HealthcareOrganization>
{
}

public sealed class HealthcareOrganizationRepository : GenericRepository<HealthcareOrganization>, IHealthcareOrganizationRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public HealthcareOrganizationRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
