namespace PeakLims.Domain.Users.Dtos;

using PeakLims.Dtos;

public sealed class UserParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
