namespace PeakLims.Domain.AccessionAttachments.Features;

using PeakLims.Domain.AccessionAttachments.Dtos;
using PeakLims.Domain.AccessionAttachments.Services;
using PeakLims.Wrappers;
using PeakLims.Exceptions;
using PeakLims.Resources;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using Microsoft.EntityFrameworkCore;
using MediatR;
using PeakLims.Services.External;
using QueryKit;
using QueryKit.Configuration;

public static class GetAccessionAttachmentList
{
    public sealed record Query(AccessionAttachmentParametersDto QueryParameters) : IRequest<PagedList<AccessionAttachmentDto>>;

    public sealed class Handler : IRequestHandler<Query, PagedList<AccessionAttachmentDto>>
    {
        private readonly IAccessionAttachmentRepository _accessionAttachmentRepository;
        private readonly IHeimGuardClient _heimGuard;
        private readonly IFileStorage _fileStorage;

        public Handler(IAccessionAttachmentRepository accessionAttachmentRepository, IHeimGuardClient heimGuard, IFileStorage fileStorage)
        {
            _accessionAttachmentRepository = accessionAttachmentRepository;
            _heimGuard = heimGuard;
            _fileStorage = fileStorage;
        }

        public async Task<PagedList<AccessionAttachmentDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadAccessionAttachments);

            var collection = _accessionAttachmentRepository.Query().AsNoTracking();

            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder,
                Configuration = queryKitConfig
            };
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToAccessionAttachmentDtoQueryable(_fileStorage);

            return await PagedList<AccessionAttachmentDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}