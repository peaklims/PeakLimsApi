namespace PeakLims.Domain.PeakOrganizations.Dtos;

using PeakLims.Resources;

public sealed class PeakOrganizationParametersDto : BasePaginationParameters
{
    public string? Filters { get; set; }
    public string? SortOrder { get; set; }
}
