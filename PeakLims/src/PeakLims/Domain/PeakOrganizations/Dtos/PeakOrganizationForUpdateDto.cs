namespace PeakLims.Domain.PeakOrganizations.Dtos;

using Destructurama.Attributed;

public sealed record PeakOrganizationForUpdateDto
{
    public string Name { get; set; }
}
