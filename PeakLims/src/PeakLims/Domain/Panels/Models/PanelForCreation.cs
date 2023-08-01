namespace PeakLims.Domain.Panels.Models;

public sealed class PanelForCreation
{
    public string PanelCode { get; set; }
    public string PanelName { get; set; }
    public string Type { get; set; }
    public int Version { get; set; }
    public string Status { get; set; }

}
