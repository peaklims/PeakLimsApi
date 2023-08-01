namespace PeakLims.Domain.Accessions.Features;

using Ardalis.Specification;
using PeakLims.Domain.Accessions.Dtos;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Wrappers;
using SharedKernel.Exceptions;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using QueryKit.Configuration;

public static class GetAccessionList
{
    public sealed class Query : IRequest<PagedList<AccessionDto>>
    {
        public readonly AccessionParametersDto QueryParameters;

        public Query(AccessionParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PagedList<AccessionDto>>
    {
        private readonly IAccessionRepository _accessionRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionRepository accessionRepository, IHeimGuardClient heimGuard)
        {
            _accessionRepository = accessionRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<AccessionDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadAccessions);
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var accessionSpecification = new AccessionWorklistSpecification(request.QueryParameters, queryKitData);
            var accessionList = await _accessionRepository.ListAsync(accessionSpecification, cancellationToken);
            var totalAccessionCount = await _accessionRepository.TotalCount(cancellationToken);

            return new PagedList<AccessionDto>(accessionList, 
                totalAccessionCount,
                request.QueryParameters.PageNumber, 
                request.QueryParameters.PageSize);
        }

        private sealed class AccessionWorklistSpecification : Specification<Accession, AccessionDto>
        {
            public AccessionWorklistSpecification(AccessionParametersDto parameters, QueryKitData queryKitData)
            {
                Query.ApplyQueryKit(queryKitData)
                    .Paginate(parameters.PageNumber, parameters.PageSize)
                    .Select(x => x.ToAccessionDto())
                    .AsNoTracking();
            }
        }
    }
}