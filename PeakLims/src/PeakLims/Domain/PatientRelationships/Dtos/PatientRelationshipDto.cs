namespace PeakLims.Domain.PatientRelationships.Dtos;

public sealed record PatientRelationshipDto
{
    public Guid Id { get; set; }
    public PatientData FromPatient { get; set; }
    public PatientData ToPatient { get; set; }
    public string FromRelationship { get; set; }
    public string ToRelationship { get; set; }
    public string? Notes { get; set; }
    public bool IsBidirectional { get; set; }

    public sealed record PatientData
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}