namespace PeakLims.Domain.Samples.DomainEvents;

public sealed class SampleCreated : DomainEvent
{
    public Sample Sample { get; set; } 
}
            