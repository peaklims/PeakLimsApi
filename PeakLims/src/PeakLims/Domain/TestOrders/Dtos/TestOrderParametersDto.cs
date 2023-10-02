namespace PeakLims.Domain.TestOrders.Dtos;

using PeakLims.Dtos;

public sealed class TestOrderParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
