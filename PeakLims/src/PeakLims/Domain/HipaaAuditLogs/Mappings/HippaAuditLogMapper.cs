namespace PeakLims.Domain.HipaaAuditLogs.Mappings;

using PeakLims.Domain.HipaaAuditLogs.Dtos;
using PeakLims.Domain.HipaaAuditLogs.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class HipaaAuditLogMapper
{
    [MapperIgnoreTarget(nameof(HipaaAuditLogForCreation.OrganizationId))]
    private static partial HipaaAuditLogForCreation ToHipaaAuditLogForCreation(this HipaaAuditLogForCreationDto hipaaAuditLogForCreationDto);

    [MapperIgnoreTarget(nameof(HipaaAuditLogForCreation.OrganizationId))]
    public static HipaaAuditLogForCreation ToHipaaAuditLogForCreation(this HipaaAuditLogForCreationDto containerForCreationDto,
        Guid organizationId)
        => containerForCreationDto.ToHipaaAuditLogForCreation()! with { OrganizationId = organizationId };
    
    public static partial HipaaAuditLogForUpdate ToHipaaAuditLogForUpdate(this HipaaAuditLogForUpdateDto hipaaAuditLogForUpdateDto);
    public static partial HipaaAuditLogDto ToHipaaAuditLogDto(this HipaaAuditLog hipaaAuditLog);
    public static partial IQueryable<HipaaAuditLogDto> ToHipaaAuditLogDtoQueryable(this IQueryable<HipaaAuditLog> hipaaAuditLog);
}