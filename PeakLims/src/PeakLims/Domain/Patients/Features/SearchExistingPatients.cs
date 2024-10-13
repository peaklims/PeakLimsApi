namespace PeakLims.Domain.Patients.Features;

using Exceptions;
using PeakLims.Domain.Patients.Dtos;
using PeakLims.Domain.Patients.Services;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using QueryKit.Configuration;

public static class SearchExistingPatients
{
    public sealed record Query(PatientParametersDto QueryParameters) : IRequest<PagedList<PatientSearchResultDto>>;

    public sealed class Handler : IRequestHandler<Query, PagedList<PatientSearchResultDto>>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPatientRepository patientRepository, IHeimGuardClient heimGuard)
        {
            _patientRepository = patientRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<PatientSearchResultDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            // var queryKitConfig = new CustomQueryKitConfiguration(config =>
            // {
            //     config.Property<Patient>(x => x.Accessions).HasQueryName("status");
            // });
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var collection = _patientRepository.Query()
                .Include(x => x.Accessions)
                .AsNoTracking();
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToPatientSearchResultDtoQueryable();

            return await PagedList<PatientSearchResultDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}