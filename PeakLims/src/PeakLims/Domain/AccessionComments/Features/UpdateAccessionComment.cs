namespace PeakLims.Domain.AccessionComments.Features;

using Exceptions;
using PeakLims.Domain.AccessionComments.Services;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;

public static class UpdateAccessionComment
{
    public sealed record Command(Guid AccessionCommentId, string Comment, string UserIdentifier) : IRequest;

    public sealed class Handler(IAccessionCommentRepository accessionCommentRepository, IUnitOfWork unitOfWork,
            IHeimGuardClient heimGuard)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accessionCommentToUpdate = await accessionCommentRepository
                .GetById(request.AccessionCommentId, cancellationToken: cancellationToken);
            
            accessionCommentToUpdate.Update(request.Comment, request.UserIdentifier, out var newComment, out var archivedComment);
            await accessionCommentRepository.Add(newComment, cancellationToken);
            accessionCommentRepository.Update(archivedComment);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}