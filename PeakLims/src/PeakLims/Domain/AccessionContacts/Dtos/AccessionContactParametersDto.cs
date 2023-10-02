namespace PeakLims.Domain.AccessionContacts.Dtos;

using PeakLims.Dtos;

public sealed class AccessionContactParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
