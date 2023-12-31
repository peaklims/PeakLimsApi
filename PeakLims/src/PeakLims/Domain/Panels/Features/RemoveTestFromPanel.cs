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
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid PanelId;
        public readonly Guid TestId;

        public Command(Guid panelId, Guid testId)
        {
            PanelId = panelId;
            TestId = testId;
        }
    }

    public sealed class Handler(IPanelRepository panelRepository, ITestRepository testRepository,
            IHeimGuardClient heimGuard, ITestOrderRepository testOrderRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanRemoveTestFromPanel);

            var panel = await panelRepository.GetById(request.PanelId, true, cancellationToken);
            var test = await testRepository.GetById(request.TestId, true, cancellationToken);
            panel.RemoveTest(test, testOrderRepository);
            await unitOfWork.CommitChanges(cancellationToken);
            
            return true;
        }
    }
}