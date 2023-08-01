namespace PeakLims.Domain.Users.Features;

using Ardalis.Specification;
using PeakLims.Domain.Users.Dtos;
using PeakLims.Domain.Users.Services;
using PeakLims.Wrappers;
using SharedKernel.Exceptions;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using QueryKit.Configuration;

public static class GetUserList
{
    public sealed class Query : IRequest<PagedList<UserDto>>
    {
        public readonly UserParametersDto QueryParameters;

        public Query(UserParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PagedList<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IUserRepository userRepository, IHeimGuardClient heimGuard)
        {
            _userRepository = userRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<UserDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadUsers);
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var userSpecification = new UserWorklistSpecification(request.QueryParameters, queryKitData);
            var userList = await _userRepository.ListAsync(userSpecification, cancellationToken);
            var totalUserCount = await _userRepository.TotalCount(cancellationToken);

            return new PagedList<UserDto>(userList, 
                totalUserCount,
                request.QueryParameters.PageNumber, 
                request.QueryParameters.PageSize);
        }

        private sealed class UserWorklistSpecification : Specification<User, UserDto>
        {
            public UserWorklistSpecification(UserParametersDto parameters, QueryKitData queryKitData)
            {
                Query.ApplyQueryKit(queryKitData)
                    .Paginate(parameters.PageNumber, parameters.PageSize)
                    .Select(x => x.ToUserDto())
                    .AsNoTracking();
            }
        }
    }
}