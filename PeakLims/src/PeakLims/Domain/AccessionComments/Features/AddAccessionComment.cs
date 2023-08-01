namespace PeakLims.Domain.AccessionComments.Features;

using PeakLims.Domain.AccessionComments.Services;
using PeakLims.Domain.AccessionComments;
using PeakLims.Domain.AccessionComments.Dtos;
using PeakLims.Domain.AccessionComments.Models;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddAccessionComment
{
    public sealed class Command : IRequest<AccessionCommentDto>
    {
        public readonly AccessionCommentForCreationDto AccessionCommentToAdd;

        public Command(AccessionCommentForCreationDto accessionCommentToAdd)
        {
            AccessionCommentToAdd = accessionCommentToAdd;
        }
    }

    public sealed class Handler : IRequestHandler<Command, AccessionCommentDto>
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

        public async Task<AccessionCommentDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddAccessionComments);

            var accessionCommentToAdd = request.AccessionCommentToAdd.ToAccessionCommentForCreation();
            var accessionComment = AccessionComment.Create(accessionCommentToAdd);

            await _accessionCommentRepository.Add(accessionComment, cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);

            return accessionComment.ToAccessionCommentDto();
        }
    }
}