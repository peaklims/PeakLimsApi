namespace PeakLims.Domain.Samples.Dtos;

using SharedKernel.Dtos;

public sealed class SampleParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
