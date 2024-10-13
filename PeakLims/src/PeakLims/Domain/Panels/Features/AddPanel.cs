namespace PeakLims.Domain.Panels.Features;

using Exceptions;
using PeakLims.Domain.Panels.Services;
using PeakLims.Domain.Panels;
using PeakLims.Domain.Panels.Dtos;
using PeakLims.Domain.Panels.Models;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddPanel
{
    public sealed record Command(PanelForCreationDto PanelToAdd) : IRequest<PanelDto>;

    public sealed class Handler(IPanelRepository panelRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        : IRequestHandler<Command, PanelDto>
    {

        public async Task<PanelDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var panelToAdd = request.PanelToAdd.ToPanelForCreation(currentUserService.GetOrganizationId());
            var panel = Panel.Create(panelToAdd);

            await panelRepository.Add(panel, cancellationToken);
            await unitOfWork.CommitChanges(cancellationToken);

            return panel.ToPanelDto();
        }
    }
}