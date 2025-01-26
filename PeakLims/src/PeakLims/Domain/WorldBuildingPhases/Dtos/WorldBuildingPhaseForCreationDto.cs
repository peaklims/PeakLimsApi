namespace PeakLims.Domain.WorldBuildingPhases.Dtos;

using Destructurama.Attributed;

public sealed record WorldBuildingPhaseForCreationDto
{
    public string Name { get; set; }
    public string Status { get; set; }
    public string ResultData { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public int StepNumber { get; set; }
    public string Comments { get; set; }
    public string ErrorMessage { get; set; }
    public string SpecialRequests { get; set; }
    public int AttemptNumber { get; set; }
}
