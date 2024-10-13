namespace PeakLims.Domain.PeakOrganizations.Dtos;

using Destructurama.Attributed;

public sealed record PeakOrganizationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
