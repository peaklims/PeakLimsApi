namespace PeakLims.Domain.TestOrders.Features;

using Ardalis.Specification;
using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.Domain.TestOrders.Services;
using PeakLims.Wrappers;
using SharedKernel.Exceptions;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using QueryKit.Configuration;

public static class GetTestOrderList
{
    public sealed class Query : IRequest<PagedList<TestOrderDto>>
    {
        public readonly TestOrderParametersDto QueryParameters;

        public Query(TestOrderParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PagedList<TestOrderDto>>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(ITestOrderRepository testOrderRepository, IHeimGuardClient heimGuard)
        {
            _testOrderRepository = testOrderRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<TestOrderDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadTestOrders);
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var testOrderSpecification = new TestOrderWorklistSpecification(request.QueryParameters, queryKitData);
            var testOrderList = await _testOrderRepository.ListAsync(testOrderSpecification, cancellationToken);
            var totalTestOrderCount = await _testOrderRepository.TotalCount(cancellationToken);

            return new PagedList<TestOrderDto>(testOrderList, 
                totalTestOrderCount,
                request.QueryParameters.PageNumber, 
                request.QueryParameters.PageSize);
        }

        private sealed class TestOrderWorklistSpecification : Specification<TestOrder, TestOrderDto>
        {
            public TestOrderWorklistSpecification(TestOrderParametersDto parameters, QueryKitData queryKitData)
            {
                Query.ApplyQueryKit(queryKitData)
                    .Paginate(parameters.PageNumber, parameters.PageSize)
                    .Select(x => x.ToTestOrderDto())
                    .AsNoTracking();
            }
        }
    }
}