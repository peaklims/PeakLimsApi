namespace PeakLims.Domain.PanelOrders.Models;

using Destructurama.Attributed;

public sealed class PanelOrderForUpdate
{
    public string CancellationReason { get; set; }
    public string CancellationComments { get; set; }

}