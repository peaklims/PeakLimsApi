namespace PeakLims.Domain.PanelOrders.Dtos;

using PeakLims.Resources;

public sealed class PanelOrderParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
