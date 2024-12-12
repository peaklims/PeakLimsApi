namespace PeakLims.Domain.PatientRelationships.Dtos;

public sealed record AddPatientRelationshipDto
{
    public Guid FromPatientId { get; init; }
    public Guid ToPatientId { get; init; }
    public string FromRelationship { get; init; }
    public string ToRelationship { get; init; }
    public bool IsConfirmedBidirectional { get; init; }
    public string? Notes { get; init; }
}