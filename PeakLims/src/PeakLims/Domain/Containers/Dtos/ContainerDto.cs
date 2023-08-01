namespace PeakLims.Domain.Containers.Dtos;

public sealed class ContainerDto
{
    public Guid Id { get; set; }
    public string UsedFor { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }
}
