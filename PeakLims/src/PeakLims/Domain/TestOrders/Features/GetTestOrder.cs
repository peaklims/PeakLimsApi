namespace PeakLims.Domain.TestOrders.Features;

using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.Domain.TestOrders.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class GetTestOrder
{
    public sealed class Query : IRequest<TestOrderDto>
    {
        public readonly Guid Id;

        public Query(Guid id)
        {
            Id = id;
        }
    }

    public sealed class Handler : IRequestHandler<Query, TestOrderDto>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(ITestOrderRepository testOrderRepository, IHeimGuardClient heimGuard)
        {
            _testOrderRepository = testOrderRepository;
            _heimGuard = heimGuard;
        }

        public async Task<TestOrderDto> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadTestOrders);

            var result = await _testOrderRepository.GetById(request.Id, cancellationToken: cancellationToken);
            return result.ToTestOrderDto();
        }
    }
}