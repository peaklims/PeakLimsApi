namespace PeakLims.Domain.HealthcareOrganizationContacts.Dtos;

public sealed class HealthcareOrganizationContactForCreationDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Npi { get; set; }
    public Guid HealthcareOrganizationId { get; set; }
}
