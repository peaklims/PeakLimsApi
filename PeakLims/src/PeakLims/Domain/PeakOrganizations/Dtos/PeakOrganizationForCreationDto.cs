namespace PeakLims.Domain.PeakOrganizations.Dtos;

using Destructurama.Attributed;

public sealed record PeakOrganizationForCreationDto
{
    public string Name { get; set; }
}
