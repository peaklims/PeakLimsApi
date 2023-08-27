namespace PeakLims.Domain.AccessionContacts.Dtos;

using SharedKernel.Dtos;

public sealed class AccessionContactParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
