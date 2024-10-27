namespace PeakLims.Domain.Containers.Features;

using Databases;
using Exceptions;
using PeakLims.Domain.Containers;
using MediatR;
using SampleTypes;

public static class AddDefaultContainers
{
    public sealed record Command(Guid OrganizationId) : IRequest;

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var containerList = new List<Container>();
            foreach (var sampleTypeName in SampleType.ListNames())
            {
                var sampleType = SampleType.Of(sampleTypeName);
                var defaultContainers = sampleType.GetDefaultContainers(request.OrganizationId);
                containerList.AddRange(defaultContainers);
            }
            
            await dbContext.Containers.AddRangeAsync(containerList, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}