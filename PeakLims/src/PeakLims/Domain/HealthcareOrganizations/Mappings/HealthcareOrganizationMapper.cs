namespace PeakLims.Domain.HealthcareOrganizations.Mappings;

using PeakLims.Domain.HealthcareOrganizations.Dtos;
using PeakLims.Domain.HealthcareOrganizations.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class HealthcareOrganizationMapper
{
    public static partial HealthcareOrganizationForCreation ToHealthcareOrganizationForCreation(this HealthcareOrganizationForCreationDto healthcareOrganizationForCreationDto);
    public static partial HealthcareOrganizationForUpdate ToHealthcareOrganizationForUpdate(this HealthcareOrganizationForUpdateDto healthcareOrganizationForUpdateDto);
    public static partial HealthcareOrganizationDto ToHealthcareOrganizationDto(this HealthcareOrganization healthcareOrganization);
    public static partial IQueryable<HealthcareOrganizationDto> ToHealthcareOrganizationDtoQueryable(this IQueryable<HealthcareOrganization> healthcareOrganization);
}