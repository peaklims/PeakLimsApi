namespace PeakLims.Domain.Patients.Services;

using PeakLims.Domain.Patients;
using PeakLims.Databases;
using PeakLims.Services;

public interface IPatientRepository : IGenericRepository<Patient>
{
}

public sealed class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    private readonly PeakLimsDbContext _dbContext;

    public PatientRepository(PeakLimsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
