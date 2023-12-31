namespace PeakLims.Domain.PanelOrders.Models;

using Destructurama.Attributed;

public sealed class PanelOrderForCreation
{
    public string Status { get; set; }
    public string CancellationReason { get; set; }
    public string CancellationComments { get; set; }

}
