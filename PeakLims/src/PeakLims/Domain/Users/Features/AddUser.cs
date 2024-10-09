namespace PeakLims.Domain.Users.Features;

using Databases;
using Dtos;
using Exceptions;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddUser
{
    public sealed record Command(UserForCreationDto UserToAdd, bool SkipPermissions = false) : IRequest<UserDto>;

    public sealed class Handler(PeakLimsDbContext dbContext, IHeimGuardClient heimGuard)
        : IRequestHandler<Command, UserDto>
    {
        public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
        {
            if(!request.SkipPermissions)
                await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddUsers);

            var userToAdd = request.UserToAdd.ToUserForCreation();
            var user = User.Create(userToAdd);
            await dbContext.Users.AddAsync(user, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            var userAdded = await dbContext.Users.GetById(user.Id, cancellationToken);
            return userAdded.ToUserDto();
        }
    }
}
