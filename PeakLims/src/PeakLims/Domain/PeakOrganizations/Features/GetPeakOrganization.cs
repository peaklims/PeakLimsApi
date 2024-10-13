namespace PeakLims.Domain.PeakOrganizations.Features;

using PeakLims.Domain.PeakOrganizations.Dtos;
using PeakLims.Databases;
using PeakLims.Exceptions;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GetPeakOrganization
{
    public sealed record Query(Guid PeakOrganizationId) : IRequest<PeakOrganizationDto>;

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Query, PeakOrganizationDto>
    {
        public async Task<PeakOrganizationDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await dbContext.PeakOrganizations
                .AsNoTracking()
                .GetById(request.PeakOrganizationId, cancellationToken);
            return result.ToPeakOrganizationDto();
        }
    }
}