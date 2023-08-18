namespace PeakLims.Domain.Accessions.Features;

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
using Microsoft.EntityFrameworkCore;
using QueryKit;
using QueryKit.Configuration;

public static class GetAccessionWorklist
{
    public sealed record Query(AccessionParametersDto QueryParameters) : IRequest<PagedList<AccessionWorklistDto>>;

    public sealed class Handler : IRequestHandler<Query, PagedList<AccessionWorklistDto>>
    {
        private readonly IAccessionRepository _accessionRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionRepository accessionRepository, IHeimGuardClient heimGuard)
        {
            _accessionRepository = accessionRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<AccessionWorklistDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadAccessions);
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };
            
            var collection = _accessionRepository.Query()
                .Include(x => x.Patient)
                .Include(x => x.TestOrders)
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