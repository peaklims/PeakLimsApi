namespace PeakLims.Domain.AccessionComments;

using SharedKernel.Exceptions;
using PeakLims.Domain.AccessionComments.DomainEvents;
using System.ComponentModel.DataAnnotations;
using AccessionCommentStatuses;
using PeakLims.Domain.Accessions;
using ValidationException = SharedKernel.Exceptions.ValidationException;

public class AccessionComment : BaseEntity
{
    public string Comment { get; private set; }

    public AccessionCommentStatus Status { get; private set; }

    public Accession Accession { get; private set; }

    public AccessionComment ParentComment { get; private set; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete

    
    public static AccessionComment Create(Accession accession, string commentText)
    {
        GuardCommentNotEmptyOrNull(commentText);
        
        var newAccessionComment = new AccessionComment
        {
            Comment = commentText,
            Accession = accession,
            ParentComment = null,
            Status = AccessionCommentStatus.Active()
        };

        newAccessionComment.QueueDomainEvent(new AccessionCommentCreated(){ AccessionComment = newAccessionComment });
        
        return newAccessionComment;
    }

    public void Update(string commentText, out AccessionComment newComment, out AccessionComment archivedComment)
    {
        GuardCommentNotEmptyOrNull(commentText);
        newComment = new AccessionComment
        {
            Comment = commentText,
            Accession = Accession,
            ParentComment = null,
            Status = AccessionCommentStatus.Active()
        };

        Status = AccessionCommentStatus.Archived();
        ParentComment = newComment;
        archivedComment = this;
        
        QueueDomainEvent(new AccessionCommentUpdated(){ Id = Id });
        newComment.QueueDomainEvent(new AccessionCommentCreated(){ AccessionComment = newComment });
    }

    private static void GuardCommentNotEmptyOrNull(string commentText)
    {
        ValidationException.ThrowWhenNullOrEmpty(commentText, "Please provide a valid comment.");
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected AccessionComment() { } // For EF + Mocking
}