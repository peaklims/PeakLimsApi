namespace PeakLims.Domain.HealthcareOrganizations.Models;

public sealed record HealthcareOrganizationForCreation
{
    public string Name { get; set; }
    public Guid OrganizationId { get; set; }
    public string KnownDomain { get; set; }
}
