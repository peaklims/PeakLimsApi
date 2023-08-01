namespace PeakLims.Domain.RolePermissions.Dtos;

using SharedKernel.Dtos;

public sealed class RolePermissionParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
