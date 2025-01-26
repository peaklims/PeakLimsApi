namespace PeakLims.Domain.WorldBuildingPhases.Mappings;

using PeakLims.Domain.WorldBuildingPhases.Dtos;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class WorldBuildingPhaseMapper
{
    public static partial WorldBuildingPhaseDto ToWorldBuildingPhaseDto(this WorldBuildingPhase worldBuildingPhase);
    public static partial IQueryable<WorldBuildingPhaseDto> ToWorldBuildingPhaseDtoQueryable(this IQueryable<WorldBuildingPhase> worldBuildingPhase);
}