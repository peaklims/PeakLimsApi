namespace PeakLims.Domain.HipaaAuditLogs.Models;

using Destructurama.Attributed;

public sealed record HipaaAuditLogForCreation
{
    public string Concept { get; set; }
    public string Data { get; set; }
    public string ActionBy { get; set; }
    public string Action { get; set; }
    public Guid Identifier { get; set; }
    public Guid OrganizationId { get; set; }
}
