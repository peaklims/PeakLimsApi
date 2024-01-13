namespace PeakLims.Domain.HipaaAuditLogs.Dtos;

using Destructurama.Attributed;

public sealed record HipaaAuditLogForUpdateDto
{
    public string Concept { get; set; }
    public string Data { get; set; }
    public string ActionBy { get; set; }
    public string Action { get; set; }
    public Guid Identifier { get; set; }
}
