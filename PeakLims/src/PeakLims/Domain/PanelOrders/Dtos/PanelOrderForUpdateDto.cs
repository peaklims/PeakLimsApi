namespace PeakLims.Domain.PanelOrders.Dtos;

using Destructurama.Attributed;

public sealed record PanelOrderForUpdateDto
{
    public string Status { get; set; }
    public string CancellationReason { get; set; }
    public string CancellationComments { get; set; }

}
