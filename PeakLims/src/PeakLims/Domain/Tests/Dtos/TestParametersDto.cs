namespace PeakLims.Domain.Tests.Dtos;

using Resources;

public sealed class TestParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
