namespace PeakLims.Domain.Users.Features;

using Databases;
using Exceptions;
using HeimGuard;
using MediatR;
using Roles;

public static class RemoveUserRole
{
    public sealed record Command(Guid UserId, string Role) : IRequest;

    public sealed class Handler(PeakLimsDbContext dbContext,
        IHeimGuardClient heimGuard) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanRemoveUserRoles);
            var user = await dbContext.GetUserAggregate().GetById(request.UserId, cancellationToken);

            var roleToRemove = user.RemoveRole(new Role(request.Role));
            dbContext.UserRoles.Remove(roleToRemove);
            dbContext.Update(user);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}