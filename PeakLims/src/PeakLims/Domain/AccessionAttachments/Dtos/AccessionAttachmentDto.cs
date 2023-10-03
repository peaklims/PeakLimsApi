namespace PeakLims.Domain.AccessionAttachments.Dtos;

using Destructurama.Attributed;

public sealed record AccessionAttachmentDto
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string S3Bucket { get; set; }
    public string S3Key { get; set; }
    
    [LogMasked]
    public string Filename { get; set; }
    public string Comments { get; set; }
    public string PreSignedUrl { get; set; }
}
