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

public static class GetPatientList
{
    public sealed record Query(PatientParametersDto QueryParameters) : IRequest<PagedList<PatientDto>>;

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
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var collection = _patientRepository.Query().AsNoTracking();
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToPatientDtoQueryable();

            return await PagedList<PatientDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}