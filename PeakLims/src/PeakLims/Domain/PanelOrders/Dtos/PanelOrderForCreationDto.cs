namespace PeakLims.Domain.PanelOrders.Dtos;

using Destructurama.Attributed;

public sealed record PanelOrderForCreationDto
{
    public string CancellationReason { get; set; }
    public string CancellationComments { get; set; }

}
