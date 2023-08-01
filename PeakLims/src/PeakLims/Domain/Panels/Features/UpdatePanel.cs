namespace PeakLims.Domain.Panels.Features;

using PeakLims.Domain.Panels;
using PeakLims.Domain.Panels.Dtos;
using PeakLims.Domain.Panels.Services;
using PeakLims.Services;
using PeakLims.Domain.Panels.Models;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdatePanel
{
    public sealed class Command : IRequest
    {
        public readonly Guid Id;
        public readonly PanelForUpdateDto UpdatedPanelData;

        public Command(Guid id, PanelForUpdateDto updatedPanelData)
        {
            Id = id;
            UpdatedPanelData = updatedPanelData;
        }
    }

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IPanelRepository _panelRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPanelRepository panelRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _panelRepository = panelRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdatePanels);

            var panelToUpdate = await _panelRepository.GetById(request.Id, cancellationToken: cancellationToken);
            var panelToAdd = request.UpdatedPanelData.ToPanelForUpdate();
            panelToUpdate.Update(panelToAdd);

            _panelRepository.Update(panelToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}