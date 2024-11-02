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

public static class AddTestToPanel
{
    public sealed record Command(Guid PanelId, Guid TestId, int TestEntries) : IRequest;

    public sealed class Handler(
        IPanelRepository panelRepository,
        ITestRepository testRepository,
        ITestOrderRepository testOrderRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var panel = await panelRepository.GetById(request.PanelId, cancellationToken: cancellationToken);
            var test = await testRepository.GetById(request.TestId, cancellationToken: cancellationToken);
            panel.AddTest(test, testOrderRepository, request.TestEntries);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}