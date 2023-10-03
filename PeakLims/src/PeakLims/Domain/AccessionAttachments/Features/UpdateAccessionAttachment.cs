namespace PeakLims.Domain.AccessionAttachments.Features;

using PeakLims.Domain.AccessionAttachments;
using PeakLims.Domain.AccessionAttachments.Dtos;
using PeakLims.Domain.AccessionAttachments.Services;
using PeakLims.Services;
using PeakLims.Domain.AccessionAttachments.Models;
using PeakLims.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateAccessionAttachment
{
    public sealed record Command(Guid AccessionAttachmentId, AccessionAttachmentForUpdateDto UpdatedAccessionAttachmentData) : IRequest;

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
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateAccessionAttachments);

            var accessionAttachmentToUpdate = await _accessionAttachmentRepository.GetById(request.AccessionAttachmentId, cancellationToken: cancellationToken);
            var accessionAttachmentToAdd = request.UpdatedAccessionAttachmentData.ToAccessionAttachmentForUpdate();
            accessionAttachmentToUpdate.Update(accessionAttachmentToAdd);

            _accessionAttachmentRepository.Update(accessionAttachmentToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}