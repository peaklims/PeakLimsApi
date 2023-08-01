namespace PeakLims.Domain.Accessions.DomainEvents;

public sealed class AccessionCreated : DomainEvent
{
    public Accession Accession { get; set; } 
}
            