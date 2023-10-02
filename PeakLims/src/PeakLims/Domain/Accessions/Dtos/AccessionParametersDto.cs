namespace PeakLims.Domain.Accessions.Dtos;

using PeakLims.Dtos;

public sealed class AccessionParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
