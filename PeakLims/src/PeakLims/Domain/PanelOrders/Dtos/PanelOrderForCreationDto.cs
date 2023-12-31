namespace PeakLims.Domain.PanelOrders.Dtos;

using Destructurama.Attributed;

public sealed record PanelOrderForCreationDto
{
    public string Status { get; set; }
    public string CancellationReason { get; set; }
    public string CancellationComments { get; set; }

}
