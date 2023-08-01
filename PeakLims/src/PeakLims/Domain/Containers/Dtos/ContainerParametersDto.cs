namespace PeakLims.Domain.Containers.Dtos;

using SharedKernel.Dtos;

public sealed class ContainerParametersDto : BasePaginationParameters
{
    public string Filters { get; set; }
    public string SortOrder { get; set; }
}
