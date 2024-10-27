namespace PeakLims.Domain.TestOrders.Features;

using Dtos;
using Exceptions;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PanelStatuses;
using PeakLims.Domain;
using PeakLims.Domain.Panels.Mappings;
using PeakLims.Domain.Panels.Services;
using PeakLims.Domain.Tests.Mappings;
using PeakLims.Domain.Tests.Services;
using TestStatuses;

public static class GetOrderablePanelsAndTests
{
    public sealed record Query() : IRequest<OrderablePanelsAndTestsDto>;

    public sealed class Handler(
        IPanelRepository panelRepository,
        ITestRepository testRepository,
        IHeimGuardClient heimGuard)
        : IRequestHandler<Query, OrderablePanelsAndTestsDto>
    {
        private readonly IHeimGuardClient _heimGuard = heimGuard;

        public async Task<OrderablePanelsAndTestsDto> Handle(Query request, CancellationToken cancellationToken)
        {
            
            var tests = await testRepository.Query()
                .AsNoTracking()
                .Where(x => x.Status == TestStatus.Active().Value)
                .ToOrderableTestQueryable()
                .ToListAsync(cancellationToken);
            
            var panels = await panelRepository.Query()
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