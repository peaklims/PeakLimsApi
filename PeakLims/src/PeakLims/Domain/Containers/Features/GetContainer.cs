namespace PeakLims.Domain.Containers.Features;

using Exceptions;
using PeakLims.Domain.Containers.Dtos;
using PeakLims.Domain.Containers.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class GetContainer
{
    public sealed class Query : IRequest<ContainerDto>
    {
        public readonly Guid Id;

        public Query(Guid id)
        {
            Id = id;
        }
    }

    public sealed class Handler : IRequestHandler<Query, ContainerDto>
    {
        private readonly IContainerRepository _containerRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IContainerRepository containerRepository, IHeimGuardClient heimGuard)
        {
            _containerRepository = containerRepository;
            _heimGuard = heimGuard;
        }

        public async Task<ContainerDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await _containerRepository.GetById(request.Id, cancellationToken: cancellationToken);
            return result.ToContainerDto();
        }
    }
}