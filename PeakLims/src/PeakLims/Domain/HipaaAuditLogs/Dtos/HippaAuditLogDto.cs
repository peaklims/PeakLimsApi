namespace PeakLims.Domain.HipaaAuditLogs.Dtos;

using Destructurama.Attributed;

public sealed record HipaaAuditLogDto
{
    public Guid Id { get; set; }
    public string Concept { get; set; }
    public string Data { get; set; }
    public string ActionBy { get; set; }
    public string Action { get; set; }
    public Guid Identifier { get; set; }
}
