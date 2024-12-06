namespace PeakLims.Domain.HealthcareOrganizationContacts.Models;

public sealed class HealthcareOrganizationContactForCreation
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Title { get; set; }
    public string Email { get; set; }
    public string Npi { get; set; }

}
