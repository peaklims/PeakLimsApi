namespace PeakLims.Domain.Containers.Dtos;

using Resources;

public sealed class ContainerParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
