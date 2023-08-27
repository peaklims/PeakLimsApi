namespace PeakLims.Domain.Accessions.Dtos;

public sealed record EditableAccessionDto
{
    public Guid Id { get; set; }
    public string AccessionNumber { get; set; }
    public string Status { get; set; }
    public PatientDto Patient { get; set; } = null!;
    public Guid? OrganizationId { get; set; }

    public sealed record PatientDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public int? Age { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string Race { get; set; }
        public string Ethnicity { get; set; }
        public string InternalId { get; set; }
    }
}
