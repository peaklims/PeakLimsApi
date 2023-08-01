namespace PeakLims.Domain.Patients.Features;

using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.Dtos;
using PeakLims.Domain.Patients.Services;
using PeakLims.Services;
using PeakLims.Domain.Patients.Models;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdatePatient
{
    public sealed class Command : IRequest
    {
        public readonly Guid Id;
        public readonly PatientForUpdateDto UpdatedPatientData;

        public Command(Guid id, PatientForUpdateDto updatedPatientData)
        {
            Id = id;
            UpdatedPatientData = updatedPatientData;
        }
    }

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPatientRepository patientRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _patientRepository = patientRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdatePatients);

            var patientToUpdate = await _patientRepository.GetById(request.Id, cancellationToken: cancellationToken);
            var patientToAdd = request.UpdatedPatientData.ToPatientForUpdate();
            patientToUpdate.Update(patientToAdd);

            _patientRepository.Update(patientToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}