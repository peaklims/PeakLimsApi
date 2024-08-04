namespace PeakLims.Domain.Accessions.Features;

using Exceptions;
using PeakLims.Domain.Accessions.Dtos;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Wrappers;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using QueryKit.Configuration;

public static class GetAccessionWorklist
{
    public sealed record Query(AccessionParametersDto QueryParameters) : IRequest<PagedList<AccessionWorklistDto>>;

    public sealed class Handler(IAccessionRepository accessionRepository, IHeimGuardClient heimGuard)
        : IRequestHandler<Query, PagedList<AccessionWorklistDto>>
    {
        public async Task<PagedList<AccessionWorklistDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadAccessions);

            var queryKitConfig = new QueryKitConfiguration(config =>
            {
                config.Property<Accession>(x => x.Status.Value).HasQueryName("status");
                config.Property<Accession>(x => x.HealthcareOrganization.Name).HasQueryName("organizationName");
            });
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };
            
            var collection = accessionRepository.Query()
                .Include(x => x.Patient)
                .Include(x => x.TestOrders)
                .Include(x => x.HealthcareOrganization)
                .AsNoTracking();
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToAccessionWorklistDtoQueryable();

            return await PagedList<AccessionWorklistDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}