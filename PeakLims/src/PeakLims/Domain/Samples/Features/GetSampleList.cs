namespace PeakLims.Domain.Samples.Features;

using Ardalis.Specification;
using PeakLims.Domain.Samples.Dtos;
using PeakLims.Domain.Samples.Services;
using PeakLims.Wrappers;
using SharedKernel.Exceptions;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using QueryKit.Configuration;

public static class GetSampleList
{
    public sealed class Query : IRequest<PagedList<SampleDto>>
    {
        public readonly SampleParametersDto QueryParameters;

        public Query(SampleParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PagedList<SampleDto>>
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(ISampleRepository sampleRepository, IHeimGuardClient heimGuard)
        {
            _sampleRepository = sampleRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<SampleDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadSamples);
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var sampleSpecification = new SampleWorklistSpecification(request.QueryParameters, queryKitData);
            var sampleList = await _sampleRepository.ListAsync(sampleSpecification, cancellationToken);
            var totalSampleCount = await _sampleRepository.TotalCount(cancellationToken);

            return new PagedList<SampleDto>(sampleList, 
                totalSampleCount,
                request.QueryParameters.PageNumber, 
                request.QueryParameters.PageSize);
        }

        private sealed class SampleWorklistSpecification : Specification<Sample, SampleDto>
        {
            public SampleWorklistSpecification(SampleParametersDto parameters, QueryKitData queryKitData)
            {
                Query.ApplyQueryKit(queryKitData)
                    .Paginate(parameters.PageNumber, parameters.PageSize)
                    .Select(x => x.ToSampleDto())
                    .AsNoTracking();
            }
        }
    }
}