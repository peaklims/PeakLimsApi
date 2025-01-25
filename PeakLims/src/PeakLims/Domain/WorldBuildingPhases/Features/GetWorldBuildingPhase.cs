namespace PeakLims.Domain.WorldBuildingPhases.Features;

using PeakLims.Domain.WorldBuildingPhases.Dtos;
using PeakLims.Databases;
using PeakLims.Exceptions;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GetWorldBuildingPhase
{
    public sealed record Query(Guid WorldBuildingPhaseId) : IRequest<WorldBuildingPhaseDto>;

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Query, WorldBuildingPhaseDto>
    {
        public async Task<WorldBuildingPhaseDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await dbContext.WorldBuildingPhases
                .AsNoTracking()
                .GetById(request.WorldBuildingPhaseId, cancellationToken);
            return result.ToWorldBuildingPhaseDto();
        }
    }
}