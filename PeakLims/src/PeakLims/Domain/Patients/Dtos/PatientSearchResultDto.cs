namespace PeakLims.Domain.Patients.Dtos;

public sealed record PatientSearchResultDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public string Sex { get; set; }
    public string InternalId { get; set; }
    public List<Accession> Accessions { get; set; }
    
    public sealed record Accession
    {
        public Guid Id { get; set; }
        public string AccessionNumber { get; set; }
    }
}
