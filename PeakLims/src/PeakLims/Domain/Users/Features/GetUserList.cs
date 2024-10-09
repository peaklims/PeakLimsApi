namespace PeakLims.Domain.Users.Features;

using Databases;
using Dtos;
using Exceptions;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using QueryKit.Configuration;
using Resources;
using Wrappers;

public static class GetUserList
{
    public sealed record Query(UserParametersDto QueryParameters) : IRequest<PagedList<UserDto>>;

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Query, PagedList<UserDto>>
    {
        public async Task<PagedList<UserDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var collection = dbContext.Users.AsNoTracking();

            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder,
                Configuration = queryKitConfig
            };
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToUserDtoQueryable();

            return await PagedList<UserDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}