namespace PeakLims.Domain.Samples;

using SharedKernel.Exceptions;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Samples.Models;
using PeakLims.Domain.Samples.DomainEvents;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using PeakLims.Domain.Containers;
using PeakLims.Domain.Containers.Models;
using SampleTypes;

public class Sample : BaseEntity
{
    public string SampleNumber { get; }

    public string Status { get; private set; }

    public SampleType Type { get; private set; }

    public decimal? Quantity { get; private set; }

    public DateOnly? CollectionDate { get; private set; }

    public DateOnly? ReceivedDate { get; private set; }

    public string CollectionSite { get; private set; }

    public Sample ParentSample { get; private set; }

    public Container Container { get; private set; }

    public Patient Patient { get; }

    public IReadOnlyCollection<TestOrder> TestOrders { get; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static Sample Create(SampleForCreation sampleForCreation)
    {
        var newSample = new Sample();

        newSample.Status = sampleForCreation.Status;
        newSample.Type = SampleType.Of(sampleForCreation.Type);
        newSample.Quantity = sampleForCreation.Quantity;
        newSample.CollectionDate = sampleForCreation.CollectionDate;
        newSample.ReceivedDate = sampleForCreation.ReceivedDate;
        newSample.CollectionSite = sampleForCreation.CollectionSite;

        newSample.QueueDomainEvent(new SampleCreated(){ Sample = newSample });
        
        return newSample;
    }

    public Sample Update(SampleForUpdate sampleForUpdate)
    {
        Status = sampleForUpdate.Status;
        Type = SampleType.Of(sampleForUpdate.Type);
        Quantity = sampleForUpdate.Quantity;
        CollectionDate = sampleForUpdate.CollectionDate;
        ReceivedDate = sampleForUpdate.ReceivedDate;
        CollectionSite = sampleForUpdate.CollectionSite;

        QueueDomainEvent(new SampleUpdated(){ Id = Id });
        return this;
    }

    public Sample SetContainer(Container container)
    {
        ValidationException.ThrowWhenNull(container, $"Invalid Container.");
        if (!container.CanStore(Type))
            throw new ValidationException(nameof(Sample),
                $"A {container.Type} container is used to store {container.UsedFor.Value} samples, not {Type.Value}.");
        if (!container.Status.IsActive())
            throw new ValidationException(nameof(Sample),
                $"Only active containers can be added to a sample.");
        
        Container = container;
        
        QueueDomainEvent(new SampleUpdated(){ Id = Id });
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected Sample() { } // For EF + Mocking
}
