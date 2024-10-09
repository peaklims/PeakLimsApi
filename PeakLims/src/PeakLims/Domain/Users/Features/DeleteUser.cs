namespace PeakLims.Domain.Users.Features;

using Databases;
using Exceptions;
using HeimGuard;
using MediatR;

public static class DeleteUser
{
    public sealed record Command(Guid UserId) : IRequest;

    public sealed class Handler(PeakLimsDbContext dbContext, IHeimGuardClient heimGuard)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanDeleteUsers);

            var recordToDelete = await dbContext.Users.GetById(request.UserId, cancellationToken: cancellationToken);
            dbContext.Remove(recordToDelete);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}