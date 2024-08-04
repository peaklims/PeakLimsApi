namespace PeakLims.Domain.AccessionContacts.Features;

using Exceptions;
using PeakLims.Domain.AccessionContacts.Dtos;
using PeakLims.Domain.AccessionContacts.Services;
using PeakLims.Wrappers;
using PeakLims.Resources;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using Microsoft.EntityFrameworkCore;
using MediatR;
using QueryKit;
using QueryKit.Configuration;

public static class GetAccessionContactList
{
    public sealed record Query(AccessionContactParametersDto QueryParameters) : IRequest<PagedList<AccessionContactDto>>;

    public sealed class Handler : IRequestHandler<Query, PagedList<AccessionContactDto>>
    {
        private readonly IAccessionContactRepository _accessionContactRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionContactRepository accessionContactRepository, IHeimGuardClient heimGuard)
        {
            _accessionContactRepository = accessionContactRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<AccessionContactDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var collection = _accessionContactRepository.Query().AsNoTracking();

            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder,
                Configuration = queryKitConfig
            };
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToAccessionContactDtoQueryable();

            return await PagedList<AccessionContactDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}