namespace PeakLims.Domain.Patients.Features;

using Ardalis.Specification;
using PeakLims.Domain.Patients.Dtos;
using PeakLims.Domain.Patients.Services;
using PeakLims.Wrappers;
using SharedKernel.Exceptions;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using QueryKit.Configuration;

public static class GetPatientList
{
    public sealed class Query : IRequest<PagedList<PatientDto>>
    {
        public readonly PatientParametersDto QueryParameters;

        public Query(PatientParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PagedList<PatientDto>>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPatientRepository patientRepository, IHeimGuardClient heimGuard)
        {
            _patientRepository = patientRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<PatientDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadPatients);
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var patientSpecification = new PatientWorklistSpecification(request.QueryParameters, queryKitData);
            var patientList = await _patientRepository.ListAsync(patientSpecification, cancellationToken);
            var totalPatientCount = await _patientRepository.TotalCount(cancellationToken);

            return new PagedList<PatientDto>(patientList, 
                totalPatientCount,
                request.QueryParameters.PageNumber, 
                request.QueryParameters.PageSize);
        }

        private sealed class PatientWorklistSpecification : Specification<Patient, PatientDto>
        {
            public PatientWorklistSpecification(PatientParametersDto parameters, QueryKitData queryKitData)
            {
                Query.ApplyQueryKit(queryKitData)
                    .Paginate(parameters.PageNumber, parameters.PageSize)
                    .Select(x => x.ToPatientDto())
                    .AsNoTracking();
            }
        }
    }
}