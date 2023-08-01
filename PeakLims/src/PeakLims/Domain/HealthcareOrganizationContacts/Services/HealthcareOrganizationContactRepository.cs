namespace PeakLims.Domain.HealthcareOrganizationContacts.Services;

using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Databases;
using PeakLims.Services;

public interface IHealthcareOrganizationContactRepository : IGenericRepository<HealthcareOrganizationContact>
{
}

public sealed class HealthcareOrganizationContactRepository : GenericRepository<HealthcareOrganizationContact>, IHealthcareOrganizationContactRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public HealthcareOrganizationContactRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
