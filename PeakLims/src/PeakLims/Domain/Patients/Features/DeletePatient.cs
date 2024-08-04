namespace PeakLims.Domain.Patients.Features;

using DomainEvents;
using Exceptions;
using PeakLims.Domain.Patients.Services;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;

public static class DeletePatient
{
    public sealed record Command(Guid Id) : IRequest;

    public sealed class Handler(
        IPatientRepository patientRepository,
        IUnitOfWork unitOfWork,
        IHeimGuardClient heimGuard)
        : IRequestHandler<Command>
    {

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var recordToDelete = await patientRepository.GetById(request.Id, cancellationToken: cancellationToken);
            patientRepository.Remove(recordToDelete);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}