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
    
    public class Handler : IRequestHandler<Command>
    {
        private readonly IAccessionRepository _accessionRepository;
        private readonly IAccessionAttachmentRepository _accessionAttachmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;
        private readonly IFileStorage _fileStorage;

        public Handler(IAccessionRepository accessionRepository, IAccessionAttachmentRepository accessionAttachmentRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, IFileStorage fileStorage)
        {
            _accessionRepository = accessionRepository;
            _accessionAttachmentRepository = accessionAttachmentRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _fileStorage = fileStorage;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUploadAccessionAttachments);
            
            var accession = await _accessionRepository.GetById(request.AccessionId, cancellationToken: cancellationToken);
            var accessionAttachment = new AccessionAttachment();
            await accessionAttachment.UploadFile(request.File, _fileStorage);
            accession.AddAccessionAttachment(accessionAttachment);
            
            await _accessionAttachmentRepository.Add(accessionAttachment, cancellationToken);
            _accessionRepository.Update(accession);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}