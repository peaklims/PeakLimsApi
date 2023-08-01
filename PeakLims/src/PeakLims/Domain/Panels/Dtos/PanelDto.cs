namespace PeakLims.Domain.Panels.Dtos;

public sealed class PanelDto
{
    public Guid Id { get; set; }
    public string PanelCode { get; set; }
    public string PanelName { get; set; }
    public string Type { get; set; }
    public int Version { get; set; }
    public string Status { get; set; }

}
