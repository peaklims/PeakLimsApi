namespace PeakLims.Domain.Accessions.Features;

using Databases;
using Exceptions;
using PeakLims.Domain.Accessions.Dtos;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Services.External;

public static class GetAccessionPageView
{
    public sealed record Query(Guid AccessionId) : IRequest<AccessionPageViewDto>;

    public sealed class Handler(
        PeakLimsDbContext dbContext,
        IFileStorage fileStorage)
        : IRequestHandler<Query, AccessionPageViewDto>
    {
        public async Task<AccessionPageViewDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var accession = (await dbContext.GetAccessionAggregate()
                .Include(x => x.AccessionAttachments)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken));
            return accession.ToEditableAccessionDto(fileStorage);
        }
    }
}