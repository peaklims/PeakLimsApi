namespace PeakLims.Domain.Panels.Features;

using HeimGuard;
using MediatR;
using PeakLims.Domain;
using PeakLims.Domain.Panels.Services;
using PeakLims.Services;
using SharedKernel.Exceptions;

public static class ActivatePanel
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid Id;

        public Command(Guid accessionId)
        {
            Id = accessionId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
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

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanActivatePanels);

            var panelToUpdate = await _panelRepository.GetById(request.Id, cancellationToken: cancellationToken);
            panelToUpdate.Activate();
            return await _unitOfWork.CommitChanges(cancellationToken) >= 1;
        }
    }
}