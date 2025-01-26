namespace PeakLims.Domain.WorldBuildingAttempts.Features;

using PeakLims.Databases;
using PeakLims.Domain.WorldBuildingAttempts;
using PeakLims.Domain.WorldBuildingAttempts.Dtos;
using PeakLims.Services;
using PeakLims.Exceptions;
using Mappings;
using MediatR;

public static class AddWorldBuildingAttempt
{
    public sealed record Command(WorldBuildingAttemptForCreationDto WorldBuildingAttemptToAdd) : IRequest<WorldBuildingAttemptDto>;

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Command, WorldBuildingAttemptDto>
    {
        public async Task<WorldBuildingAttemptDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var worldBuildingAttempt = WorldBuildingAttempt.CreateStandardWorld();

            await dbContext.WorldBuildingAttempts.AddAsync(worldBuildingAttempt, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return worldBuildingAttempt.ToWorldBuildingAttemptDto();
        }
    }
}