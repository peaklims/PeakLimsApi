namespace PeakLims.Domain.HealthcareOrganizations.Dtos;

public sealed class HealthcareOrganizationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string KnownDomain { get; set; }
}
