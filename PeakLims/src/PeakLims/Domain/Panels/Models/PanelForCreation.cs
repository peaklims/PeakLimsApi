namespace PeakLims.Domain.Panels.Models;

public sealed record PanelForCreation
{
    public string PanelCode { get; set; }
    public string PanelName { get; set; }
    public string Type { get; set; }
    public Guid OrganizationId { get; set; }
}
