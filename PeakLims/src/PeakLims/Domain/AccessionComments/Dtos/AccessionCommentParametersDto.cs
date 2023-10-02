namespace PeakLims.Domain.AccessionComments.Dtos;

using PeakLims.Dtos;

public sealed class AccessionCommentParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
