namespace PeakLims.Domain.Accessions.Dtos;

using Patients.Dtos;

public record SetPatientDto(Guid AccessionId, Guid? PatientId, PatientForCreationDto PatientForCreation);