namespace PeakLims.Domain.Patients.Mappings;

using PeakLims.Domain.Patients.Dtos;
using PeakLims.Domain.Patients.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class PatientMapper
{
    public static partial PatientForCreation ToPatientForCreation(this PatientForCreationDto patientForCreationDto);
    public static partial PatientForUpdate ToPatientForUpdate(this PatientForUpdateDto patientForUpdateDto);
    public static partial PatientDto ToPatientDto(this Patient patient);
    public static partial IQueryable<PatientDto> ToPatientDtoQueryable(this IQueryable<Patient> patient);
}