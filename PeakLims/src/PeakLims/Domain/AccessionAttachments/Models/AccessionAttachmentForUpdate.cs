namespace PeakLims.Domain.AccessionAttachments.Models;

using Destructurama.Attributed;

public sealed class AccessionAttachmentForUpdate
{
    public string Type { get; set; }
    public string Comments { get; set; }
    public string DisplayName { get; set; }
}
