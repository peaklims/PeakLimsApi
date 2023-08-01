namespace PeakLims.Domain;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();
    
    public DateTime CreatedOn { get; private set; }
    
    public string CreatedBy { get; private set; }
    
    public DateTime? LastModifiedOn { get; private set; }
    
    public string LastModifiedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    
    [NotMapped]
    public List<DomainEvent> DomainEvents { get; } = new List<DomainEvent>();

    public void UpdateCreationProperties(DateTime createdOn, string createdBy)
    {
        CreatedOn = createdOn;
        CreatedBy = createdBy;
    }
    
    public void UpdateModifiedProperties(DateTime? lastModifiedOn, string lastModifiedBy)
    {
        LastModifiedOn = lastModifiedOn;
        LastModifiedBy = lastModifiedBy;
    }
    
    public void UpdateIsDeleted(bool isDeleted)
    {
        IsDeleted = isDeleted;
    }
    
    public void QueueDomainEvent(DomainEvent @event)
    {
        if(!DomainEvents.Contains(@event))
            DomainEvents.Add(@event);
    }
}