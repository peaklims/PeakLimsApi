namespace PeakLims.Domain.Patients.Services;

using DomainEvents;
using MediatR;
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

    public override void Remove(Patient patient)
    {
        patient.DomainEvents.Add(new PatientDeleted { Id = patient.Id, ActionBy = patient.LastModifiedBy});
        _dbContext.Patients.Remove(patient);
    }
}
