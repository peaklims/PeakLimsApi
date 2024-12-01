namespace PeakLims.Domain.Samples.Dtos;

public sealed class SampleForUpdateDto
{
    public string Type { get; set; }
    public decimal? Quantity { get; set; }
    public DateOnly? CollectionDate { get; set; }
    public DateOnly ReceivedDate { get; set; }
    public string CollectionSite { get; set; }
    public Guid? ContainerId { get; set; }
    public string ExternalId { get; set; }
}
