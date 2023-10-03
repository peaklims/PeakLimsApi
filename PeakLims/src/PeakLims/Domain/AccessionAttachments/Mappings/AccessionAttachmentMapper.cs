namespace PeakLims.Domain.AccessionAttachments.Mappings;

using PeakLims.Domain.AccessionAttachments.Dtos;
using PeakLims.Domain.AccessionAttachments.Models;
using PeakLims.Services.External;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class AccessionAttachmentMapper
{
    public static partial AccessionAttachmentForUpdate ToAccessionAttachmentForUpdate(this AccessionAttachmentForUpdateDto accessionAttachmentForUpdateDto);

    public static AccessionAttachmentDto ToAccessionAttachmentDto(this AccessionAttachment accessionAttachment, IFileStorage fileStorage)
    {
        return new AccessionAttachmentDto()
        {
            Id = accessionAttachment.Id,
            Type = accessionAttachment.Type.Value,
            S3Bucket = accessionAttachment.S3Bucket,
            S3Key = accessionAttachment.S3Key.Value,
            Filename = accessionAttachment.Filename,
            Comments = accessionAttachment.Comments,
            PreSignedUrl = accessionAttachment.GetPreSignedUrl(fileStorage)
        };
    }
    public static IQueryable<AccessionAttachmentDto> ToAccessionAttachmentDtoQueryable(this IQueryable<AccessionAttachment> accessionAttachment, IFileStorage fileStorage)
    {
        return accessionAttachment.Select(x => new AccessionAttachmentDto()
        {
            Id = x.Id,
            Type = x.Type.Value,
            S3Bucket = x.S3Bucket,
            S3Key = x.S3Key.Value,
            Filename = x.Filename,
            Comments = x.Comments,
            PreSignedUrl = x.GetPreSignedUrl(fileStorage)
        });
    }
}