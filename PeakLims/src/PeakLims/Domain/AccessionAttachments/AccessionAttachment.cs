namespace PeakLims.Domain.AccessionAttachments;

using PeakLims.Domain.Accessions;
using Destructurama.Attributed;
using PeakLims.Exceptions;
using PeakLims.Domain.AccessionAttachments.Models;
using PeakLims.Domain.AccessionAttachments.DomainEvents;
using PeakLims.Services.External;
using Resources;
using S3Keys;
using ValidationException = Exceptions.ValidationException;

public class AccessionAttachment : BaseEntity
{
    public AccessionAttachmentType Type { get; set; }

    public string S3Bucket { get; private set; }

    public S3Key S3Key { get; private set; }

    [LogMasked]
    public string Filename { get; private set; }
    
    [LogMasked]
    public string DisplayName { get; private set; }

    public string Comments { get; private set; }

    public Accession Accession { get; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static AccessionAttachment Create(AccessionAttachmentForCreation accessionAttachmentForCreation)
    {
        var newAccessionAttachment = new AccessionAttachment();

        newAccessionAttachment.Type = AccessionAttachmentType.Of(accessionAttachmentForCreation.Type);
        newAccessionAttachment.Comments = accessionAttachmentForCreation.Comments;
        newAccessionAttachment.DisplayName = accessionAttachmentForCreation.DisplayName;

        newAccessionAttachment.QueueDomainEvent(new AccessionAttachmentCreated(){ AccessionAttachment = newAccessionAttachment });
        
        return newAccessionAttachment;
    }

    public AccessionAttachment Update(AccessionAttachmentForUpdate accessionAttachmentForUpdate)
    {
        Type = AccessionAttachmentType.Of(accessionAttachmentForUpdate.Type);
        Comments = accessionAttachmentForUpdate.Comments;
        DisplayName = accessionAttachmentForUpdate.DisplayName ?? Filename;

        QueueDomainEvent(new AccessionAttachmentUpdated(){ Id = Id });
        return this;
    }
    
    public async Task<AccessionAttachment> UploadFile(IFormFile formFile, IFileStorage fileStorage) 
    {
        ValidationException.ThrowWhenNull(formFile, $"Please provide a file to upload.");

        var fileExtension = Path.GetExtension(formFile.FileName);
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        
        var key = $"accession-attachments/{fileName}";

        S3Bucket = Consts.S3Buckets.AccessionAttachments;
        S3Key = S3Key.Of(key);
        Filename = formFile.FileName;
        DisplayName ??= Filename;
        
        await fileStorage.UploadFileAsync(S3Bucket, S3Key.Value, formFile);

        QueueDomainEvent(new AccessionAttachmentUpdated(){ Id = Id });
        return this;
    }
    
    public string GetPreSignedUrl(IFileStorage fileStorage)
    {
        ValidationException.ThrowWhenNull(S3Bucket, $"The S3 bucket is not set for this accession attachment.");
        ValidationException.ThrowWhenNull(S3Key, $"The S3 key is not set for this accession attachment.");
        return fileStorage.GetPreSignedUrl(S3Bucket, S3Key.Value);
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    public AccessionAttachment() { } // For EF + Mocking
}

