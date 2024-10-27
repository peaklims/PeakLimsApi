namespace PeakLims.Domain.Containers;

using PeakLims.Domain.Samples;
using PeakLims.Domain.Containers.Models;
using PeakLims.Domain.Containers.DomainEvents;
using PeakOrganizations;
using SampleTypes;

public class Container : BaseEntity
{
    public SampleType UsedFor { get; private set; }

    
    public PeakOrganization Organization { get; }
    public Guid OrganizationId { get; private set;  }

    public string Type { get; private set; }
    public bool CanStore(SampleType sampleType) => UsedFor == sampleType;

    public IReadOnlyCollection<Sample> Samples { get; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static Container Create(ContainerForCreation containerForCreation)
    {
        var newContainer = new Container();

        newContainer.UsedFor = SampleType.Of(containerForCreation.UsedFor);
        newContainer.Type = containerForCreation.Type;
        newContainer.OrganizationId = containerForCreation.OrganizationId;

        newContainer.QueueDomainEvent(new ContainerCreated(){ Container = newContainer });
        
        return newContainer;
    }

    public Container Update(ContainerForUpdate containerForUpdate)
    {
        UsedFor = SampleType.Of(containerForUpdate.UsedFor);
        Type = containerForUpdate.Type;

        QueueDomainEvent(new ContainerUpdated(){ Id = Id });
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected Container() { } // For EF + Mocking
}
