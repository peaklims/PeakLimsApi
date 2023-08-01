namespace PeakLims.Domain.HealthcareOrganizationContacts.Dtos;

public sealed class HealthcareOrganizationContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Npi { get; set; }

}
