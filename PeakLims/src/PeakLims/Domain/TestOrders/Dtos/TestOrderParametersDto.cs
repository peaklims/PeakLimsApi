namespace PeakLims.Domain.TestOrders.Dtos;

using Resources;

public sealed class TestOrderParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
