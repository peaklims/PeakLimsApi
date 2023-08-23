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
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.Age) }, new[] { nameof(PatientDto.Age) })]
    public static partial PatientDto ToPatientDto(this Patient patient);
    
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.DateOfBirth) }, new[] { nameof(PatientDto.DateOfBirth) })]
    [MapProperty(new[] { nameof(Patient.Lifespan), nameof(Patient.Lifespan.Age) }, new[] { nameof(PatientDto.Age) })]
    public static partial IQueryable<PatientDto> ToPatientDtoQueryable(this IQueryable<Patient> patient);

    public static IQueryable<PatientSearchResultDto> ToPatientSearchResultDtoQueryable(this IQueryable<Patient> patient)
    {
        return patient?.Select(x => x == null 
            ? default 
            : new PatientSearchResultDto() 
            { 
                Id = x.Id, 
                FirstName = x.FirstName, 
                LastName = x.LastName, 
                DateOfBirth = x.Lifespan.DateOfBirth,
                Age = x.Lifespan.Age,
                Sex = x.Sex.Value,
                InternalId = x.InternalId,
                Accessions = x.Accessions.Select(x => new PatientSearchResultDto.Accession()
                {
                    Id = x.Id,
                    AccessionNumber = x.AccessionNumber
                }).ToList()
            });
    }
}