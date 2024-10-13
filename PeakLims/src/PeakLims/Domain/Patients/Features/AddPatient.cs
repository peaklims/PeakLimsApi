namespace PeakLims.Domain.Patients.Features;

using PeakLims.Domain.Patients.Services;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.Dtos;
using PeakLims.Services;
using Mappings;
using MediatR;

public static class AddPatient
{
    public sealed record Command(PatientForCreationDto PatientToAdd) : IRequest<PatientDto>;

    public sealed class Handler(
        IPatientRepository patientRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
        : IRequestHandler<Command, PatientDto>
    {
        public async Task<PatientDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var patientToAdd = request.PatientToAdd.ToPatientForCreation(currentUserService.GetOrganizationId());
            var patient = Patient.Create(patientToAdd);

            await patientRepository.Add(patient, cancellationToken);
            await unitOfWork.CommitChanges(cancellationToken);

            return patient.ToPatientDto();
        }
    }
}