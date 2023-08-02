namespace PeakLims.Domain.AccessionComments.Dtos;

public sealed class AccessionCommentForCreationDto
{
    public string Comment { get; set; }
    public string Status { get; set; }
    public Guid AccessionId { get; set; }
}
