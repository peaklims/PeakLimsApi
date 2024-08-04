namespace PeakLims.Domain.AccessionAttachments.Features;

using PeakLims.Domain.AccessionAttachments.Services;
using PeakLims.Services;
using PeakLims.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using MediatR;

public static class DeleteAccessionAttachment
{
    public sealed record Command(Guid AccessionAttachmentId) : IRequest;

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IAccessionAttachmentRepository _accessionAttachmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionAttachmentRepository accessionAttachmentRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _accessionAttachmentRepository = accessionAttachmentRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var recordToDelete = await _accessionAttachmentRepository.GetById(request.AccessionAttachmentId, cancellationToken: cancellationToken);
            _accessionAttachmentRepository.Remove(recordToDelete);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}