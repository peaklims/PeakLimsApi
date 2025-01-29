namespace PeakLims.Domain.WorldBuildingAttempts.Mappings;

using PeakLims.Domain.WorldBuildingAttempts.Dtos;
using Riok.Mapperly.Abstractions;
using WorldBuildingPhases.Mappings;

[Mapper]
public static partial class WorldBuildingAttemptMapper
{
    [MapperIgnoreTarget(nameof(WorldBuildingAttemptDto.Phases))]
    private static partial WorldBuildingAttemptDto ToWorldBuildingAttemptDtoBase(this WorldBuildingAttempt worldBuildingAttempt);

    public static WorldBuildingAttemptDto ToWorldBuildingAttemptDto(this WorldBuildingAttempt worldBuildingAttempt)
    {
        return worldBuildingAttempt.ToWorldBuildingAttemptDtoBase() with
        {
            Phases = worldBuildingAttempt.WorldBuildingPhases.Select(x => x.ToWorldBuildingPhaseDto()).ToList()
        };
    }  
    
    public static partial IQueryable<WorldBuildingAttemptDto> ToWorldBuildingAttemptDtoQueryable(this IQueryable<WorldBuildingAttempt> worldBuildingAttempt);
}