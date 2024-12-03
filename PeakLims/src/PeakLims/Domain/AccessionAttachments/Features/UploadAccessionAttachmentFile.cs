namespace PeakLims.Domain.AccessionAttachments.Features;

using Accessions.Services;
using Exceptions;
using HeimGuard;
using MediatR;
using PeakLims.Services;
using PeakLims.Services.External;
using Services;

public class UploadAccessionAttachmentFile
{
    public record Command(Guid AccessionId, IFormFile File) : IRequest;
    
    public class Handler(
        IAccessionRepository accessionRepository,
        IAccessionAttachmentRepository accessionAttachmentRepository,
        IUnitOfWork unitOfWork,
        IFileStorage fileStorage)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = await accessionRepository.GetById(request.AccessionId, cancellationToken: cancellationToken);
            var accessionAttachment = new AccessionAttachment();
            await accessionAttachment.UploadFile(request.File, fileStorage);
            accession.AddAccessionAttachment(accessionAttachment);
            
            await accessionAttachmentRepository.Add(accessionAttachment, cancellationToken);
            accessionRepository.Update(accession);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}