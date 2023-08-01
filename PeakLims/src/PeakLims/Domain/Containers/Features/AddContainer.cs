namespace PeakLims.Domain.Containers.Features;

using PeakLims.Domain.Containers.Services;
using PeakLims.Domain.Containers;
using PeakLims.Domain.Containers.Dtos;
using PeakLims.Domain.Containers.Models;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddContainer
{
    public sealed class Command : IRequest<ContainerDto>
    {
        public readonly ContainerForCreationDto ContainerToAdd;

        public Command(ContainerForCreationDto containerToAdd)
        {
            ContainerToAdd = containerToAdd;
        }
    }

    public sealed class Handler : IRequestHandler<Command, ContainerDto>
    {
        private readonly IContainerRepository _containerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IContainerRepository containerRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _containerRepository = containerRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task<ContainerDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddContainers);

            var containerToAdd = request.ContainerToAdd.ToContainerForCreation();
            var container = Container.Create(containerToAdd);

            await _containerRepository.Add(container, cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);

            return container.ToContainerDto();
        }
    }
}