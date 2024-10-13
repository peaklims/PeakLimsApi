namespace PeakLims.Domain.Panels.Dtos;

using Resources;

public sealed class PanelParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
