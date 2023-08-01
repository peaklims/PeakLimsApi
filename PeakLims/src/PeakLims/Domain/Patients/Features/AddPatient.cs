namespace PeakLims.Domain.Patients.Features;

using PeakLims.Domain.Patients.Services;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.Dtos;
using PeakLims.Domain.Patients.Models;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddPatient
{
    public sealed class Command : IRequest<PatientDto>
    {
        public readonly PatientForCreationDto PatientToAdd;

        public Command(PatientForCreationDto patientToAdd)
        {
            PatientToAdd = patientToAdd;
        }
    }

    public sealed class Handler : IRequestHandler<Command, PatientDto>
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

        public async Task<PatientDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddPatients);

            var patientToAdd = request.PatientToAdd.ToPatientForCreation();
            var patient = Patient.Create(patientToAdd);

            await _patientRepository.Add(patient, cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);

            return patient.ToPatientDto();
        }
    }
}