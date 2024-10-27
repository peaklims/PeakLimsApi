namespace PeakLims.Domain.Containers.Features;

using Exceptions;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Domain;
using PeakLims.Domain.Containers.Dtos;
using PeakLims.Domain.Containers.Mappings;
using PeakLims.Domain.Containers.Services;
using SampleTypes;

public static class GetAllContainers
{
    public sealed record Query(string? SampleType) : IRequest<List<ContainerDto>>;

    public sealed class Handler(IContainerRepository containerRepository)
        : IRequestHandler<Query, List<ContainerDto>>
    {
        public async Task<List<ContainerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            return string.IsNullOrWhiteSpace(request.SampleType) 
                ? await containerRepository.Query()
                    .AsNoTracking()
                    .ToContainerDtoQueryable()
                    .ToListAsync(cancellationToken)
                : await containerRepository.Query()
                    .AsNoTracking()
                    .Where(c => c.UsedFor == SampleType.Of(request.SampleType))
                    .ToContainerDtoQueryable()
                    .ToListAsync(cancellationToken);
        }
    }
}