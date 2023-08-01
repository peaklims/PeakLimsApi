namespace PeakLims.Domain.Samples.DomainEvents;

public sealed class SampleUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            