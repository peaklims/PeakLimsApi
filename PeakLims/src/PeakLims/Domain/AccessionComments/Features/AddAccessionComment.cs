namespace PeakLims.Domain.AccessionComments.Features;

using Accessions.Services;
using Exceptions;
using PeakLims.Domain.AccessionComments.Services;
using PeakLims.Domain.AccessionComments;
using PeakLims.Domain.AccessionComments.Dtos;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddAccessionComment
{
    public sealed record Command(Guid AccessionId, string Comment) : IRequest<AccessionCommentDto>;

    public sealed class Handler(
        IAccessionCommentRepository accessionCommentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAccessionRepository accessionRepository)
        : IRequestHandler<Command, AccessionCommentDto>
    {
        public async Task<AccessionCommentDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = await accessionRepository.GetById(request.AccessionId, cancellationToken: cancellationToken);
            var accessionComment = AccessionComment.Create(accession, request.Comment, currentUserService?.UserIdentifier);
            await accessionCommentRepository.Add(accessionComment, cancellationToken);

            await unitOfWork.CommitChanges(cancellationToken);

            return accessionComment.ToAccessionCommentDto();
        }
    }
}