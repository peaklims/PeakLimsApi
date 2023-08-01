namespace PeakLims.Domain.TestOrders.Dtos;

public sealed class TestOrderForUpdateDto
{
    public string Status { get; set; }
    public DateOnly? DueDate { get; set; }
    public int? TatSnapshot { get; set; }
    public string CancellationReason { get; set; }
    public string CancellationComments { get; set; }
    public Guid? AssociatedPanelId { get; set; }

}
