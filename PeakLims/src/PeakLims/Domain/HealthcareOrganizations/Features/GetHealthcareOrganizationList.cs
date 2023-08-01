namespace PeakLims.Domain.HealthcareOrganizations.Features;

using Ardalis.Specification;
using PeakLims.Domain.HealthcareOrganizations.Dtos;
using PeakLims.Domain.HealthcareOrganizations.Services;
using PeakLims.Wrappers;
using SharedKernel.Exceptions;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using QueryKit.Configuration;

public static class GetHealthcareOrganizationList
{
    public sealed class Query : IRequest<PagedList<HealthcareOrganizationDto>>
    {
        public readonly HealthcareOrganizationParametersDto QueryParameters;

        public Query(HealthcareOrganizationParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PagedList<HealthcareOrganizationDto>>
    {
        private readonly IHealthcareOrganizationRepository _healthcareOrganizationRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IHealthcareOrganizationRepository healthcareOrganizationRepository, IHeimGuardClient heimGuard)
        {
            _healthcareOrganizationRepository = healthcareOrganizationRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<HealthcareOrganizationDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadHealthcareOrganizations);
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var healthcareOrganizationSpecification = new HealthcareOrganizationWorklistSpecification(request.QueryParameters, queryKitData);
            var healthcareOrganizationList = await _healthcareOrganizationRepository.ListAsync(healthcareOrganizationSpecification, cancellationToken);
            var totalHealthcareOrganizationCount = await _healthcareOrganizationRepository.TotalCount(cancellationToken);

            return new PagedList<HealthcareOrganizationDto>(healthcareOrganizationList, 
                totalHealthcareOrganizationCount,
                request.QueryParameters.PageNumber, 
                request.QueryParameters.PageSize);
        }

        private sealed class HealthcareOrganizationWorklistSpecification : Specification<HealthcareOrganization, HealthcareOrganizationDto>
        {
            public HealthcareOrganizationWorklistSpecification(HealthcareOrganizationParametersDto parameters, QueryKitData queryKitData)
            {
                Query.ApplyQueryKit(queryKitData)
                    .Paginate(parameters.PageNumber, parameters.PageSize)
                    .Select(x => x.ToHealthcareOrganizationDto())
                    .AsNoTracking();
            }
        }
    }
}