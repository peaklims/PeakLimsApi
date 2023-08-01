namespace PeakLims.Domain.Panels.Dtos;

using SharedKernel.Dtos;

public sealed class PanelParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
