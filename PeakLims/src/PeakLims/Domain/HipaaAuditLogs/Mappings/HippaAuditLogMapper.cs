namespace PeakLims.Domain.HipaaAuditLogs.Mappings;

using PeakLims.Domain.HipaaAuditLogs.Dtos;
using PeakLims.Domain.HipaaAuditLogs.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class HipaaAuditLogMapper
{
    public static partial HipaaAuditLogForCreation ToHipaaAuditLogForCreation(this HipaaAuditLogForCreationDto hipaaAuditLogForCreationDto);
    public static partial HipaaAuditLogForUpdate ToHipaaAuditLogForUpdate(this HipaaAuditLogForUpdateDto hipaaAuditLogForUpdateDto);
    public static partial HipaaAuditLogDto ToHipaaAuditLogDto(this HipaaAuditLog hipaaAuditLog);
    public static partial IQueryable<HipaaAuditLogDto> ToHipaaAuditLogDtoQueryable(this IQueryable<HipaaAuditLog> hipaaAuditLog);
}