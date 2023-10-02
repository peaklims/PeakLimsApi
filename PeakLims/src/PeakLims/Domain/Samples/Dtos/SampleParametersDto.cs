namespace PeakLims.Domain.Samples.Dtos;

using PeakLims.Dtos;

public sealed class SampleParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
