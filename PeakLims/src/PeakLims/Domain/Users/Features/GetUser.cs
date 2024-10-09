namespace PeakLims.Domain.Users.Features;

using Databases;
using Dtos;
using Exceptions;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GetUser
{
    public sealed record Query(Guid UserId) : IRequest<UserDto>;

    public sealed class Handler(PeakLimsDbContext dbContext
    )
        : IRequestHandler<Query, UserDto>
    {
        public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await dbContext.Users
                .AsNoTracking()
                .GetById(request.UserId, cancellationToken);
            return result.ToUserDto();
        }
    }
}