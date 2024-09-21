namespace PeakLims.Domain.Patients.Features;

using Databases;
using PeakLims.Domain.Patients.Dtos;
using PeakLims.Services;
using HeimGuard;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class UpdatePatient
{
    public sealed record Command(Guid Id, PatientForUpdateDto UpdatedPatientData) : IRequest;

    public sealed class Handler(
        PeakLimsDbContext dbContext,
        IUnitOfWork unitOfWork,
        IHeimGuardClient heimGuard)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var patientToUpdate = (await dbContext.GetPatientAggregate()
                .GetById(request.Id, cancellationToken: cancellationToken))
                .MustBeFoundOrThrow();
            
            var patientToAdd = request.UpdatedPatientData.ToPatientForUpdate();
            patientToUpdate.Update(patientToAdd);

            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}