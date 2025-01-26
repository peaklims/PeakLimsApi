namespace PeakLims.Domain.WorldBuildingAttempts.Dtos;

using Destructurama.Attributed;

public sealed record WorldBuildingAttemptDto
{
    public Guid Id { get; set; }
    public string Status { get; set; }

}
