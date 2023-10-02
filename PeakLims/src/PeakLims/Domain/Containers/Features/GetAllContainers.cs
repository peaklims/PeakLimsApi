namespace PeakLims.Domain.Containers.Features;

using Exceptions;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Domain;
using PeakLims.Domain.Containers.Dtos;
using PeakLims.Domain.Containers.Mappings;
using PeakLims.Domain.Containers.Services;

public static class GetAllContainers
{
    public sealed record Query() : IRequest<List<ContainerDto>>;

    public sealed class Handler : IRequestHandler<Query, List<ContainerDto>>
    {
        private readonly IContainerRepository _containerRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IContainerRepository containerRepository, IHeimGuardClient heimGuard)
        {
            _containerRepository = containerRepository;
            _heimGuard = heimGuard;
        }

        public async Task<List<ContainerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadContainers);
            return await _containerRepository.Query()
                .AsNoTracking()
                .ToContainerDtoQueryable()
                .ToListAsync(cancellationToken);
        }
    }
}