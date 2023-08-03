namespace PeakLims.Domain.AccessionComments.Dtos;

public sealed class AccessionCommentViewDto
{
    public List<AccessionCommentItemDto> AccessionComments { get; set; } = new List<AccessionCommentItemDto>();

    public sealed class AccessionCommentItemDto
    {
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByFirstName { get; set; }
        public string CreatedByLastName { get; set; }
        public string CreatedById { get; set; }
        public List<AccessionCommentHistoryRecordDto> History { get; set; } = new List<AccessionCommentHistoryRecordDto>();
    }
    
    public sealed class AccessionCommentHistoryRecordDto
    {
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByFirstName { get; set; }
        public string CreatedByLastName { get; set; }
        public string CreatedById { get; set; }
    }
}