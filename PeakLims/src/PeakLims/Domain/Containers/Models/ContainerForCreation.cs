namespace PeakLims.Domain.Containers.Models;

public sealed record ContainerForCreation
{
    public string UsedFor { get; set; }
    public string Type { get; set; }
    public Guid OrganizationId { get; set; }
}
