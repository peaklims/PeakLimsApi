namespace PeakLims.Domain.AccessionAttachments.Dtos;

using PeakLims.Dtos;
using PeakLims.Resources;

public sealed class AccessionAttachmentParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
