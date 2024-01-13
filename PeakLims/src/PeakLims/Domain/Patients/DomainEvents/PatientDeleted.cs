namespace PeakLims.Domain.Patients.DomainEvents;

public sealed class PatientDeleted : DomainEvent
{
    public Guid Id { get; set; } 
    public string ActionBy { get; set; }
}
            