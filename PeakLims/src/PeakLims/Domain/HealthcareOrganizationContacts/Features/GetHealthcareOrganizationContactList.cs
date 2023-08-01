namespace PeakLims.Domain.HealthcareOrganizationContacts.Features;

using Ardalis.Specification;
using PeakLims.Domain.HealthcareOrganizationContacts.Dtos;
using PeakLims.Domain.HealthcareOrganizationContacts.Services;
using PeakLims.Wrappers;
using SharedKernel.Exceptions;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using QueryKit.Configuration;

public static class GetHealthcareOrganizationContactList
{
    public sealed class Query : IRequest<PagedList<HealthcareOrganizationContactDto>>
    {
        public readonly HealthcareOrganizationContactParametersDto QueryParameters;

        public Query(HealthcareOrganizationContactParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PagedList<HealthcareOrganizationContactDto>>
    {
        private readonly IHealthcareOrganizationContactRepository _healthcareOrganizationContactRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IHealthcareOrganizationContactRepository healthcareOrganizationContactRepository, IHeimGuardClient heimGuard)
        {
            _healthcareOrganizationContactRepository = healthcareOrganizationContactRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<HealthcareOrganizationContactDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadHealthcareOrganizationContacts);
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var healthcareOrganizationContactSpecification = new HealthcareOrganizationContactWorklistSpecification(request.QueryParameters, queryKitData);
            var healthcareOrganizationContactList = await _healthcareOrganizationContactRepository.ListAsync(healthcareOrganizationContactSpecification, cancellationToken);
            var totalHealthcareOrganizationContactCount = await _healthcareOrganizationContactRepository.TotalCount(cancellationToken);

            return new PagedList<HealthcareOrganizationContactDto>(healthcareOrganizationContactList, 
                totalHealthcareOrganizationContactCount,
                request.QueryParameters.PageNumber, 
                request.QueryParameters.PageSize);
        }

        private sealed class HealthcareOrganizationContactWorklistSpecification : Specification<HealthcareOrganizationContact, HealthcareOrganizationContactDto>
        {
            public HealthcareOrganizationContactWorklistSpecification(HealthcareOrganizationContactParametersDto parameters, QueryKitData queryKitData)
            {
                Query.ApplyQueryKit(queryKitData)
                    .Paginate(parameters.PageNumber, parameters.PageSize)
                    .Select(x => x.ToHealthcareOrganizationContactDto())
                    .AsNoTracking();
            }
        }
    }
}