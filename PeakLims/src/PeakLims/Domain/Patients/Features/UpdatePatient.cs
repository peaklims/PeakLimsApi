namespace PeakLims.Domain.Patients.Features;

using Exceptions;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.Dtos;
using PeakLims.Domain.Patients.Services;
using PeakLims.Services;
using PeakLims.Domain.Patients.Models;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdatePatient
{
    public sealed record Command(Guid Id, PatientForUpdateDto UpdatedPatientData) : IRequest;

    public sealed class Handler(
        IPatientRepository patientRepository,
        IUnitOfWork unitOfWork,
        IHeimGuardClient heimGuard)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var patientToUpdate = await patientRepository.GetById(request.Id, cancellationToken: cancellationToken);
            var patientToAdd = request.UpdatedPatientData.ToPatientForUpdate();
            patientToUpdate.Update(patientToAdd);

            patientRepository.Update(patientToUpdate);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}