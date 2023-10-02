namespace PeakLims.Domain.Panels.Dtos;

using PeakLims.Dtos;

public sealed class PanelParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
