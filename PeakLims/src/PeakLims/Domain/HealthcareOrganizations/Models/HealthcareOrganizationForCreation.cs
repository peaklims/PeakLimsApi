namespace PeakLims.Domain.HealthcareOrganizations.Models;

public sealed record HealthcareOrganizationForCreation
{
    public string Name { get; set; }
    public string Email { get; set; }
    public Guid OrganizationId { get; set; }
}
