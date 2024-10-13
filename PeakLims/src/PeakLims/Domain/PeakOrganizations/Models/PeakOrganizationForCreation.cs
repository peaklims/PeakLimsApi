namespace PeakLims.Domain.PeakOrganizations.Models;

using Destructurama.Attributed;

public sealed record PeakOrganizationForCreation
{
    public string Name { get; set; }
}
