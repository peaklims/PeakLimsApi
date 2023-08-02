namespace PeakLims.Domain.Samples.Dtos;

public sealed class SampleDto
{
    public Guid Id { get; set; }
    public string SampleNumber { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }
    public decimal? Quantity { get; set; }
    public DateOnly? CollectionDate { get; set; }
    public DateOnly? ReceivedDate { get; set; }
    public string CollectionSite { get; set; }
    public Guid? ContainerId { get; set; }
}
