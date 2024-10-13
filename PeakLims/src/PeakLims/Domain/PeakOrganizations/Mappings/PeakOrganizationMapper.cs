namespace PeakLims.Domain.PeakOrganizations.Mappings;

using PeakLims.Domain.PeakOrganizations.Dtos;
using PeakLims.Domain.PeakOrganizations.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class PeakOrganizationMapper
{
    public static partial PeakOrganizationForCreation ToPeakOrganizationForCreation(this PeakOrganizationForCreationDto peakOrganizationForCreationDto);
    public static partial PeakOrganizationForUpdate ToPeakOrganizationForUpdate(this PeakOrganizationForUpdateDto peakOrganizationForUpdateDto);
    public static partial PeakOrganizationDto ToPeakOrganizationDto(this PeakOrganization peakOrganization);
    public static partial IQueryable<PeakOrganizationDto> ToPeakOrganizationDtoQueryable(this IQueryable<PeakOrganization> peakOrganization);
}