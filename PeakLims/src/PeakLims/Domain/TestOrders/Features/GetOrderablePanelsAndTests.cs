namespace PeakLims.Domain.TestOrders.Features;

using Dtos;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PanelStatuses;
using PeakLims.Domain;
using PeakLims.Domain.Panels.Mappings;
using PeakLims.Domain.Panels.Services;
using PeakLims.Domain.Tests.Mappings;
using PeakLims.Domain.Tests.Services;
using SharedKernel.Exceptions;
using TestStatuses;

public static class GetOrderablePanelsAndTests
{
    public sealed record Query() : IRequest<OrderablePanelsAndTestsDto>;

    public sealed class Handler : IRequestHandler<Query, OrderablePanelsAndTestsDto>
    {
        private readonly IPanelRepository _panelRepository;
        private readonly ITestRepository _testRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPanelRepository panelRepository, ITestRepository testRepository, IHeimGuardClient heimGuard)
        {
            _panelRepository = panelRepository;
            _testRepository = testRepository;
            _heimGuard = heimGuard;
        }

        public async Task<OrderablePanelsAndTestsDto> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadPanels);
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadTests);
            
            var tests = await _testRepository.Query()
                .AsNoTracking()
                .Where(x => x.Status == TestStatus.Active().Value)
                .ToOrderableTestQueryable()
                .ToListAsync(cancellationToken);
            
            var panels = await _panelRepository.Query()
                .AsNoTracking()
                .Where(x => x.Status == PanelStatus.Active().Value)
                .ToOrderablePanelQueryable()
                .ToListAsync(cancellationToken);

            return new OrderablePanelsAndTestsDto()
            {
                Panels = panels,
                Tests = tests
            };
        }
    }
}