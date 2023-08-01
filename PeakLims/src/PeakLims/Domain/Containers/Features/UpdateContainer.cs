namespace PeakLims.Domain.Containers.Features;

using PeakLims.Domain.Containers;
using PeakLims.Domain.Containers.Dtos;
using PeakLims.Domain.Containers.Services;
using PeakLims.Services;
using PeakLims.Domain.Containers.Models;
using SharedKernel.Exceptions;
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

    public sealed class Handler : IRequestHandler<Command>
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

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateContainers);

            var containerToUpdate = await _containerRepository.GetById(request.Id, cancellationToken: cancellationToken);
            var containerToAdd = request.UpdatedContainerData.ToContainerForUpdate();
            containerToUpdate.Update(containerToAdd);

            _containerRepository.Update(containerToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}