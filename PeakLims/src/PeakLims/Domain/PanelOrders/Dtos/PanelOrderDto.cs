namespace PeakLims.Domain.PanelOrders.Dtos;

using Destructurama.Attributed;

public sealed record PanelOrderDto
{
    public Guid Id { get; set; }
    public string CancellationReason { get; set; }
    public string CancellationComments { get; set; }

}
