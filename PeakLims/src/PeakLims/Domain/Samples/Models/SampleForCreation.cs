namespace PeakLims.Domain.Samples.Models;

public sealed class SampleForCreation
{
    public string Type { get; set; }
    public decimal? Quantity { get; set; }
    public DateOnly? CollectionDate { get; set; }
    public DateOnly? ReceivedDate { get; set; }
    public string CollectionSite { get; set; }
    public string ExternalId { get; set; }
}
