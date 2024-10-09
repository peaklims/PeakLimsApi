namespace PeakLims.Domain.Users.Features;

using Databases;
using Dtos;
using Exceptions;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateUser
{
    public sealed record Command(Guid UserId, UserForUpdateDto UpdatedUserData) : IRequest;

    public sealed class Handler(PeakLimsDbContext dbContext, IHeimGuardClient heimGuard)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateUsers);

            var userToUpdate = await dbContext.Users.GetById(request.UserId, cancellationToken);
            var userToAdd = request.UpdatedUserData.ToUserForUpdate();
            userToUpdate.Update(userToAdd);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}