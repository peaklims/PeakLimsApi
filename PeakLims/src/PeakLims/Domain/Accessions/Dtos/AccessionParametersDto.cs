namespace PeakLims.Domain.Accessions.Dtos;

using Resources;

public sealed class AccessionParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
