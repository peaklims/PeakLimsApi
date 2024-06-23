namespace PeakLims.Domain.Accessions.Features;

using Exceptions;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;

public static class SubmitAccession
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid Id;

        public Command(Guid accessionId)
        {
            Id = accessionId;
        }
    }

    public sealed class Handler(
        IAccessionRepository accessionRepository,
        IUnitOfWork unitOfWork,
        IHeimGuardClient heimGuard)
        : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanSetAccessionStatusToReadyForTesting);

            var accessionToUpdate = await accessionRepository.GetAccessionForStatusChange(request.Id, cancellationToken);
            accessionToUpdate.Submit();
            return await unitOfWork.CommitChanges(cancellationToken) >= 1;
        }
    }
}