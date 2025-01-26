namespace PeakLims.Domain.WorldBuildingPhases.Dtos;

using PeakLims.Resources;

public sealed class WorldBuildingPhaseParametersDto : BasePaginationParameters
{
    public string? Filters { get; set; }
    public string? SortOrder { get; set; }
}
