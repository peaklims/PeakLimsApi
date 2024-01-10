namespace PeakLims.Domain.AccessionComments.Features;

using Exceptions;
using PeakLims.Domain.AccessionComments;
using PeakLims.Domain.AccessionComments.Dtos;
using PeakLims.Domain.AccessionComments.Services;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateAccessionComment
{
    public sealed class Command : IRequest
    {
        public readonly Guid AccessionCommentId;
        public readonly string Comment;

        public Command(Guid accessionCommentId, string comment)
        {
            AccessionCommentId = accessionCommentId;
            Comment = comment;
        }
    }

    public sealed class Handler(IAccessionCommentRepository accessionCommentRepository, IUnitOfWork unitOfWork,
            IHeimGuardClient heimGuard)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateAccessionComments);

            var accessionCommentToUpdate = await accessionCommentRepository
                .GetById(request.AccessionCommentId, cancellationToken: cancellationToken);

            accessionCommentToUpdate.Update(request.Comment, out var newComment, out var archivedComment);
            await accessionCommentRepository.Add(newComment, cancellationToken);
            accessionCommentRepository.Update(archivedComment);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}