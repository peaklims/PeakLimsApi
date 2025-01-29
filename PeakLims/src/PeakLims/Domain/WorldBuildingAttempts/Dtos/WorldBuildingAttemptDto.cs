namespace PeakLims.Domain.WorldBuildingAttempts.Dtos;

using Destructurama.Attributed;
using WorldBuildingPhases.Dtos;

public sealed record WorldBuildingAttemptDto
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    public List<WorldBuildingPhaseDto> Phases { get; set; }

}
