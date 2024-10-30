namespace PeakLims.Domain.AccessionComments;

using PeakLims.Domain.AccessionComments.DomainEvents;
using System.ComponentModel.DataAnnotations;
using AccessionCommentStatuses;
using Exceptions;
using PeakLims.Domain.Accessions;
using Users;
using ValidationException = Exceptions.ValidationException;

public class AccessionComment : BaseEntity
{
    public string Comment { get; private set; }
    
    public string CommentedByIdentifier { get; private set; }
    
    public DateTimeOffset CommentedAt { get; private set; }

    public AccessionCommentStatus Status { get; private set; }

    public Accession Accession { get; private set; }

    public AccessionComment ParentComment { get; private set; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete

    
    public static AccessionComment Create(Accession accession, string commentText, string commentedByIdentifier, 
        DateTimeOffset? commentedAt = null)
    {
        GuardCommentNotEmptyOrNull(commentText);
        ValidationException.ThrowWhenNullOrEmpty(commentedByIdentifier, "Please provide a valid user identifier.");
        
        var newAccessionComment = new AccessionComment
        {
            Comment = commentText,
            Accession = accession,
            ParentComment = null,
            CommentedByIdentifier = commentedByIdentifier,
            CommentedAt = commentedAt ?? DateTimeOffset.UtcNow,
            Status = AccessionCommentStatus.Active()
        };

        newAccessionComment.QueueDomainEvent(new AccessionCommentCreated(){ AccessionComment = newAccessionComment });
        
        return newAccessionComment;
    }

    public void Update(string commentText, string userIdentifier, out AccessionComment newComment, out AccessionComment archivedComment)
    {
        GuardCommentNotEmptyOrNull(commentText);
        ValidationException.ThrowWhenNullOrEmpty(userIdentifier, "Please provide a valid user identifier.");
        var canBeUpdated = CanBeUpdatedByUser(userIdentifier);
        if (!canBeUpdated)
            throw new ForbiddenAccessException("Accession comment cannot be updated by this user.");
        
        newComment = new AccessionComment
        {
            Comment = commentText,
            Accession = Accession,
            ParentComment = null,
            Status = AccessionCommentStatus.Active(),
            CommentedByIdentifier = userIdentifier,
            CommentedAt = DateTimeOffset.UtcNow
        };

        Status = AccessionCommentStatus.Archived();
        ParentComment = newComment;
        archivedComment = this;
        
        QueueDomainEvent(new AccessionCommentUpdated(){ Id = Id });
        newComment.QueueDomainEvent(new AccessionCommentCreated(){ AccessionComment = newComment });
    }
    
    public bool CanBeUpdatedByUser(string userIdentifier) 
        => CommentedByIdentifier == userIdentifier;

    private static void GuardCommentNotEmptyOrNull(string commentText)
    {
        ValidationException.ThrowWhenNullOrEmpty(commentText, "Please provide a valid comment.");
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected AccessionComment() { } // For EF + Mocking
}