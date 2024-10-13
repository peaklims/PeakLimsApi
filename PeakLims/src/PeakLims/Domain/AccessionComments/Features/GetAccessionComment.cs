namespace PeakLims.Domain.AccessionComments.Features;

using Exceptions;
using PeakLims.Domain.AccessionComments.Dtos;
using PeakLims.Domain.AccessionComments.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using PeakLims.Services;

public static class GetAccessionComment
{
    public sealed record Query(Guid Id) : IRequest<AccessionCommentDto>;

    public sealed class Handler(IAccessionCommentRepository accessionCommentRepository)
        : IRequestHandler<Query, AccessionCommentDto>
    {
        public async Task<AccessionCommentDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await accessionCommentRepository.GetById(request.Id, cancellationToken: cancellationToken);
            return result.ToAccessionCommentDto();
        }
    }
}