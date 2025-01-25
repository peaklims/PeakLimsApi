namespace PeakLims.Domain.WorldBuildingAttempts.Mappings;

using PeakLims.Domain.WorldBuildingAttempts.Dtos;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class WorldBuildingAttemptMapper
{
    public static partial WorldBuildingAttemptDto ToWorldBuildingAttemptDto(this WorldBuildingAttempt worldBuildingAttempt);
    public static partial IQueryable<WorldBuildingAttemptDto> ToWorldBuildingAttemptDtoQueryable(this IQueryable<WorldBuildingAttempt> worldBuildingAttempt);
}