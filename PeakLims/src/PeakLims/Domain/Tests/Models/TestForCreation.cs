namespace PeakLims.Domain.Tests.Models;

public sealed record TestForCreation
{
    public string TestCode { get; set; }
    public string TestName { get; set; }
    public string Methodology { get; set; }
    public string Platform { get; set; }
    public int? TurnAroundTime { get; set; }
    public Guid OrganizationId { get; set; }
}
