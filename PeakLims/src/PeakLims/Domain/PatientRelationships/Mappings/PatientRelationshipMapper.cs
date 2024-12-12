namespace PeakLims.Domain.PatientRelationships.Mappings;

using Dtos;
using Patients;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.HipaaAuditLogs.Models;
using PeakLims.Domain.Patients.Dtos;
using PeakLims.Domain.Patients.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class PatientRelationshipMapper
{
    [MapperIgnoreTarget(nameof(PatientForCreation.OrganizationId))]
    private static partial PatientForCreation ToPatientForCreation(this PatientForCreationDto patientForCreationDto);

    [MapperIgnoreTarget(nameof(PatientForCreation.OrganizationId))]
    public static PatientForCreation ToPatientForCreation(this PatientForCreationDto patientForCreationDto,
        Guid organizationId)
        => patientForCreationDto.ToPatientForCreation()! with { OrganizationId = organizationId };
    
    public static partial PatientRelationshipDto.PatientData ToPatientRelationshipData(this Patient patient);
}