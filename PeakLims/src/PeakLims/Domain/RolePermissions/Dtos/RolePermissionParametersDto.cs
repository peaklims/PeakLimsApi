namespace PeakLims.Domain.RolePermissions.Dtos;

using Resources;

public sealed class RolePermissionParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
