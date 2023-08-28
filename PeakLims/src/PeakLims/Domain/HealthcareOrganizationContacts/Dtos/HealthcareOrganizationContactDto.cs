namespace PeakLims.Domain.HealthcareOrganizationContacts.Dtos;

public sealed class HealthcareOrganizationContactDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Npi { get; set; }
    public Guid HealthcareOrganizationId { get; set; }
}
