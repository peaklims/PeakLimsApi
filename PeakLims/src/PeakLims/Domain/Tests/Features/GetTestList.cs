namespace PeakLims.Domain.Tests.Features;

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
using Microsoft.EntityFrameworkCore;
using QueryKit;
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

            var collection = _testRepository.Query().AsNoTracking();
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToTestDtoQueryable();

            return await PagedList<TestDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}