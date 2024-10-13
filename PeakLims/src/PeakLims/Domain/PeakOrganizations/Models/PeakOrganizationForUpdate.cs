namespace PeakLims.Domain.PeakOrganizations.Models;

using Destructurama.Attributed;

public sealed record PeakOrganizationForUpdate
{
    public string Name { get; set; }
}
