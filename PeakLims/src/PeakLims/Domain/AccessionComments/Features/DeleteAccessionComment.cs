namespace PeakLims.Domain.AccessionComments.Features;

using PeakLims.Domain.AccessionComments.Services;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using MediatR;

public static class DeleteAccessionComment
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid Id;

        public Command(Guid accessionComment)
        {
            Id = accessionComment;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
    {
        private readonly IAccessionCommentRepository _accessionCommentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionCommentRepository accessionCommentRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _accessionCommentRepository = accessionCommentRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanDeleteAccessionComments);

            var recordToDelete = await _accessionCommentRepository.GetById(request.Id, cancellationToken: cancellationToken);

            _accessionCommentRepository.Remove(recordToDelete);
            return await _unitOfWork.CommitChanges(cancellationToken) >= 1;
        }
    }
}