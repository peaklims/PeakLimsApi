namespace PeakLims.Domain.HipaaAuditLogs.DomainEvents;

public sealed class HipaaAuditLogCreated : DomainEvent
{
    public HipaaAuditLog HipaaAuditLog { get; set; } 
}
            