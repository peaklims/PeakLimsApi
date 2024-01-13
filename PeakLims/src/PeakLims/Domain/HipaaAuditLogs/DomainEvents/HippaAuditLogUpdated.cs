namespace PeakLims.Domain.HipaaAuditLogs.DomainEvents;

public sealed class HipaaAuditLogUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            