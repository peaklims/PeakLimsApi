namespace PeakLims.Domain.WorldBuildingAttempts.Features;

using PeakLims.Domain.WorldBuildingAttempts.Dtos;
using PeakLims.Databases;
using PeakLims.Exceptions;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GetWorldBuildingAttempt
{
    public sealed record Query(Guid WorldBuildingAttemptId) : IRequest<WorldBuildingAttemptDto>;

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Query, WorldBuildingAttemptDto>
    {
        public async Task<WorldBuildingAttemptDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await dbContext.WorldBuildingAttempts
                .AsNoTracking()
                .GetById(request.WorldBuildingAttemptId, cancellationToken);
            return result.ToWorldBuildingAttemptDto();
        }
    }
}