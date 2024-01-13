namespace PeakLims.Domain.HipaaAuditLogs.Dtos;

using PeakLims.Dtos;
using PeakLims.Resources;

public sealed class HipaaAuditLogParametersDto : BasePaginationParameters
{
    public string? Filters { get; set; }
    public string? SortOrder { get; set; }
}
