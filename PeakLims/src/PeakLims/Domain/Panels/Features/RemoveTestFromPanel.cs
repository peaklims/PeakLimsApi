namespace PeakLims.Domain.Panels.Features;

using Exceptions;
using PeakLims.Domain.Panels.Dtos;
using PeakLims.Domain.Panels.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;
using PeakLims.Services;
using TestOrders.Services;
using Tests.Services;

public static class RemoveTestFromPanel
{
    public sealed record Command(Guid PanelId, Guid TestId) : IRequest;

    public sealed class Handler(IPanelRepository panelRepository, ITestRepository testRepository,
            IHeimGuardClient heimGuard, ITestOrderRepository testOrderRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanRemoveTestFromPanel);

            var panel = await panelRepository.GetById(request.PanelId, true, cancellationToken);
            var test = await testRepository.GetById(request.TestId, true, cancellationToken);
            panel.RemoveTest(test, testOrderRepository);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}