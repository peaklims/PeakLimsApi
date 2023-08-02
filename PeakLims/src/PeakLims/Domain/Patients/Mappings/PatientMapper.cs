namespace PeakLims.Domain.Patients.Mappings;

using PeakLims.Domain.Patients.Dtos;
using PeakLims.Domain.Patients.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class PatientMapper
{
    public static partial PatientForCreation ToPatientForCreation(this PatientForCreationDto patientForCreationDto);
    public static partial PatientForUpdate ToPatientForUpdate(this PatientForUpdateDto patientForUpdateDto);
    
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.DateOfBirth) }, new[] { nameof(PatientDto.DateOfBirth) })]
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.KnownAge) }, new[] { nameof(PatientDto.Age) })]
    public static partial PatientDto ToPatientDto(this Patient patient);
    
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.DateOfBirth) }, new[] { nameof(PatientDto.DateOfBirth) })]
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.KnownAge) }, new[] { nameof(PatientDto.Age) })]
    public static partial IQueryable<PatientDto> ToPatientDtoQueryable(this IQueryable<Patient> patient);
}