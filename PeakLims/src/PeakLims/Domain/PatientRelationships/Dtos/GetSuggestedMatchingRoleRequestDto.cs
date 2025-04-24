namespace PeakLims.Domain.PatientRelationships.Dtos;

public record GetSuggestedMatchingRoleRequestDto(string Relationship, Guid ToPatientId);