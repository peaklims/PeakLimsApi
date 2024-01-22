namespace PeakLims.Domain.Containers.Features;

using Exceptions;
using PeakLims.Domain.Containers;
using PeakLims.Domain.Containers.Dtos;
using PeakLims.Domain.Containers.Services;
using PeakLims.Services;
using PeakLims.Domain.Containers.Models;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateContainer
{
    public sealed class Command : IRequest
    {
        public readonly Guid Id;
        public readonly ContainerForUpdateDto UpdatedContainerData;

        public Command(Guid id, ContainerForUpdateDto updatedContainerData)
        {
            Id = id;
            UpdatedContainerData = updatedContainerData;
        }
    }

    public sealed class Handler(
        IContainerRepository containerRepository,
        IUnitOfWork unitOfWork,
        IHeimGuardClient heimGuard)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateContainers);

            var containerToUpdate = await containerRepository.GetById(request.Id, cancellationToken: cancellationToken);
            var containerToAdd = request.UpdatedContainerData.ToContainerForUpdate();
            containerToUpdate.Update(containerToAdd);

            containerRepository.Update(containerToUpdate);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}