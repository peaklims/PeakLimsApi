namespace PeakLims.Domain.Patients.Dtos;

public sealed class PatientForUpdateDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public string Sex { get; set; }
    public string Race { get; set; }
    public string Ethnicity { get; set; }

}
