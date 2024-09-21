namespace PeakLims.Domain.Patients.Mappings;

using Accessions;
using HipaaAuditLogs.Models;
using PeakLims.Domain.Patients.Dtos;
using PeakLims.Domain.Patients.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class PatientMapper
{
    public static partial PatientForCreation ToPatientForCreation(this PatientForCreationDto patientForCreationDto);
    public static partial PatientForUpdate ToPatientForUpdate(this PatientForUpdateDto patientForUpdateDto);
    
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.DateOfBirth) }, new[] { nameof(PatientDto.DateOfBirth) })]
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.Age) }, new[] { nameof(PatientDto.Age) })]
    public static partial PatientDto ToPatientDto(this Patient patient);
    
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.DateOfBirth) }, new[] { nameof(PatientDto.DateOfBirth) })]
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.Age) }, new[] { nameof(PatientDto.Age) })]
    public static partial PatientAuditLogEntry ToPatientAuditLogEntry(this Patient patient);
    
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.DateOfBirth) }, new[] { nameof(PatientDto.DateOfBirth) })]
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.Age) }, new[] { nameof(PatientDto.Age) })]
    public static partial IQueryable<PatientDto> ToPatientDtoQueryable(this IQueryable<Patient> patient);

    [MapperIgnoreTarget(nameof(PatientSearchResultDto.Accessions))]
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.DateOfBirth) }, new[] { nameof(PatientSearchResultDto.DateOfBirth) })]
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.Age) }, new[] { nameof(PatientSearchResultDto.Age) })]
    private static partial PatientSearchResultDto ToPatientSearchResultDto(this Patient accessionAttachment);
    public static PatientSearchResultDto ToPatientSearchResultDto(this Patient accessionAttachment, IEnumerable<Accession> accessions)
    {
        return accessionAttachment.ToPatientSearchResultDto()! with { Accessions = accessions.Select(x => new PatientSearchResultDto.Accession()
        {
            Id = x.Id,
            AccessionNumber = x.AccessionNumber
        }).ToList() };
    }
    
    public static IQueryable<PatientSearchResultDto> ToPatientSearchResultDtoQueryable(this IQueryable<Patient> patient)
    {
        return patient?.Select(x => x == null 
            ? default 
            : x.ToPatientSearchResultDto(x.Accessions));
    }
}