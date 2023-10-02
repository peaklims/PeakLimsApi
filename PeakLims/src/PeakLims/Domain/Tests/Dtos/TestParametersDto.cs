namespace PeakLims.Domain.Tests.Dtos;

using PeakLims.Dtos;

public sealed class TestParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
