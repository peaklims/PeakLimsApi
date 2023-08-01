namespace PeakLims.Domain.Accessions.Dtos;

using SharedKernel.Dtos;

public sealed class AccessionParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
