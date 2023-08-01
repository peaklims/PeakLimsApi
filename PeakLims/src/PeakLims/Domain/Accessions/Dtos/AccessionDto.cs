namespace PeakLims.Domain.Accessions.Dtos;

public sealed class AccessionDto
{
    public Guid Id { get; set; }
    public string AccessionNumber { get; set; }
    public string Status { get; set; }

}
