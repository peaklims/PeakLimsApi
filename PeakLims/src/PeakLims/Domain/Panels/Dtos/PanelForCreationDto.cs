namespace PeakLims.Domain.Panels.Dtos;

public sealed class PanelForCreationDto
{
    public string PanelCode { get; set; }
    public string PanelName { get; set; }
    public string Type { get; set; }
    public int Version { get; set; }
    public string Status { get; set; }

}
