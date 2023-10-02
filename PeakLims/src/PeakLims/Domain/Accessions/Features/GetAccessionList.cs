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
            
            var queryKitConfig = new QueryKitConfiguration(config =>
            {
                config.Property<Accession>(x => x.Status.Value).HasQueryName("status");
            });
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };
            
            var collection = _accessionRepository.Query().AsNoTracking();
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToAccessionDtoQueryable();

            return await PagedList<AccessionDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}