namespace PeakLims.Domain.Tests.Features;

using Ardalis.Specification;
using PeakLims.Domain.Tests.Dtos;
using PeakLims.Domain.Tests.Services;
using PeakLims.Wrappers;
using SharedKernel.Exceptions;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using QueryKit.Configuration;

public static class GetTestList
{
    public sealed class Query : IRequest<PagedList<TestDto>>
    {
        public readonly TestParametersDto QueryParameters;

        public Query(TestParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PagedList<TestDto>>
    {
        private readonly ITestRepository _testRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(ITestRepository testRepository, IHeimGuardClient heimGuard)
        {
            _testRepository = testRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<TestDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadTests);
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var testSpecification = new TestWorklistSpecification(request.QueryParameters, queryKitData);
            var testList = await _testRepository.ListAsync(testSpecification, cancellationToken);
            var totalTestCount = await _testRepository.TotalCount(cancellationToken);

            return new PagedList<TestDto>(testList, 
                totalTestCount,
                request.QueryParameters.PageNumber, 
                request.QueryParameters.PageSize);
        }

        private sealed class TestWorklistSpecification : Specification<Test, TestDto>
        {
            public TestWorklistSpecification(TestParametersDto parameters, QueryKitData queryKitData)
            {
                Query.ApplyQueryKit(queryKitData)
                    .Paginate(parameters.PageNumber, parameters.PageSize)
                    .Select(x => x.ToTestDto())
                    .AsNoTracking();
            }
        }
    }
}