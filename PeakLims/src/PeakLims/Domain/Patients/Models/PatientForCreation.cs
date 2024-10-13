namespace PeakLims.Domain.Patients.Models;

public sealed record PatientForCreation
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public string Sex { get; set; }
    public string Race { get; set; }
    public string Ethnicity { get; set; }
    public Guid OrganizationId { get; set; }
}
