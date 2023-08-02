namespace PeakLims.Domain.AccessionComments.Features;

using Accessions.Services;
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
        public readonly string Comment;
        public readonly Guid AccessionId;

        public Command( Guid accessionId, string comment)
        {
            Comment = comment;
            AccessionId = accessionId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, AccessionCommentDto>
    {
        private readonly IAccessionCommentRepository _accessionCommentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;
        private readonly IAccessionRepository _accessionRepository;

        public Handler(IAccessionCommentRepository accessionCommentRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, IAccessionRepository accessionRepository)
        {
            _accessionCommentRepository = accessionCommentRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _accessionRepository = accessionRepository;
        }

        public async Task<AccessionCommentDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddAccessionComments);

            var accession = await _accessionRepository.GetById(request.AccessionId, cancellationToken: cancellationToken);
            var accessionComment = AccessionComment.Create(accession, request.Comment);
            await _accessionCommentRepository.Add(accessionComment, cancellationToken);

            await _unitOfWork.CommitChanges(cancellationToken);

            return accessionComment.ToAccessionCommentDto();
        }
    }
}