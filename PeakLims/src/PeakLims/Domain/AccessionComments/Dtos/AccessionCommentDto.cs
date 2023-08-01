namespace PeakLims.Domain.AccessionComments.Dtos;

public sealed class AccessionCommentDto
{
    public Guid Id { get; set; }
    public string Comment { get; set; }
    public string Status { get; set; }

}
