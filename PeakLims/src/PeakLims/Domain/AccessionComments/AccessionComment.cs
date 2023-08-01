namespace PeakLims.Domain.AccessionComments;

using SharedKernel.Exceptions;
using PeakLims.Domain.AccessionComments.Models;
using PeakLims.Domain.AccessionComments.DomainEvents;
using FluentValidation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.Accessions.Models;


public class AccessionComment : BaseEntity
{
    public string Comment { get; private set; }

    public string Status { get; private set; }

    public Accession Accession { get; private set; }

    public AccessionComment ParentComment { get; private set; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static AccessionComment Create(AccessionCommentForCreation accessionCommentForCreation)
    {
        var newAccessionComment = new AccessionComment();

        newAccessionComment.Comment = accessionCommentForCreation.Comment;
        newAccessionComment.Status = accessionCommentForCreation.Status;

        newAccessionComment.QueueDomainEvent(new AccessionCommentCreated(){ AccessionComment = newAccessionComment });
        
        return newAccessionComment;
    }

    public AccessionComment Update(AccessionCommentForUpdate accessionCommentForUpdate)
    {
        Comment = accessionCommentForUpdate.Comment;
        Status = accessionCommentForUpdate.Status;

        QueueDomainEvent(new AccessionCommentUpdated(){ Id = Id });
        return this;
    }

    public AccessionComment SetAccession(Accession accession)
    {
        Accession = accession;
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected AccessionComment() { } // For EF + Mocking
}