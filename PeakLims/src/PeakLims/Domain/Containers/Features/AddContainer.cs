namespace PeakLims.Domain.Containers.Features;

using Exceptions;
using PeakLims.Domain.Containers.Services;
using PeakLims.Domain.Containers;
using PeakLims.Domain.Containers.Dtos;
using PeakLims.Domain.Containers.Models;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddContainer
{
    public sealed class Command(ContainerForCreationDto containerToAdd) : IRequest<ContainerDto>
    {
        public readonly ContainerForCreationDto ContainerToAdd = containerToAdd;
    }

    public sealed class Handler(
        IContainerRepository containerRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
        : IRequestHandler<Command, ContainerDto>
    {
        public async Task<ContainerDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var containerToAdd = request.ContainerToAdd.ToContainerForCreation(currentUserService.GetOrganizationId());
            var container = Container.Create(containerToAdd);

            await containerRepository.Add(container, cancellationToken);
            await unitOfWork.CommitChanges(cancellationToken);

            return container.ToContainerDto();
        }
    }
}