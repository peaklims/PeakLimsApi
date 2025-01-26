namespace PeakLims.Domain.WorldBuildingAttempts.Dtos;

using PeakLims.Resources;

public sealed class WorldBuildingAttemptParametersDto : BasePaginationParameters
{
    public string? Filters { get; set; }
    public string? SortOrder { get; set; }
}
