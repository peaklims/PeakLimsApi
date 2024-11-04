namespace PeakLims.Domain.Patients.Features;

using Databases;
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

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Query, PagedList<PatientSearchResultDto>>
    {
        public async Task<PagedList<PatientSearchResultDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var collection = dbContext.Patients
                .Include(x => x.Accessions)
                .AsNoTracking();
            
            // var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitConfig = new CustomQueryKitConfiguration(config =>
            {
                // config.Property<Patient>(x => x.Accessions).HasQueryName("status");
                config.Property<Patient>(x => x.Accessions.Select(y => y.AccessionNumber))
                    .HasQueryName("accessionNumber");
                config.Property<Patient>(x => x.FirstName)
                    .HasQueryName("firstName");
                config.Property<Patient>(x => x.LastName)
                    .HasQueryName("lastName");
                config.Property<Patient>(x => x.InternalId)
                    .HasQueryName("internalId");
                config.Property<Patient>(x => x.Lifespan.DateOfBirth)
                    .HasQueryName("dob");
            });
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToPatientSearchResultDtoQueryable();

            return await PagedList<PatientSearchResultDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}