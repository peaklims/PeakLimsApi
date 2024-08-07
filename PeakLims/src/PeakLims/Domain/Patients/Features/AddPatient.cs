namespace PeakLims.Domain.Patients.Features;

using Exceptions;
using PeakLims.Domain.Patients.Services;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.Dtos;
using PeakLims.Domain.Patients.Models;
using PeakLims.Services;
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

    public sealed class Handler(
        IPatientRepository patientRepository,
        IUnitOfWork unitOfWork,
        IHeimGuardClient heimGuard)
        : IRequestHandler<Command, PatientDto>
    {
        public async Task<PatientDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var patientToAdd = request.PatientToAdd.ToPatientForCreation();
            var patient = Patient.Create(patientToAdd);

            await patientRepository.Add(patient, cancellationToken);
            await unitOfWork.CommitChanges(cancellationToken);

            return patient.ToPatientDto();
        }
    }
}