namespace PeakLims.Domain.Accessions.Features;

using Exceptions;
using Patients.Services;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;
using Patients;
using Patients.Dtos;
using Patients.Mappings;

public static class SetAccessionPatient
{
    public sealed record Command(Guid AccessionId, Guid? PatientId, PatientForCreationDto PatientForCreationDto) : IRequest;

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IAccessionRepository _accessionRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionRepository accessionRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, IPatientRepository patientRepository)
        {
            _accessionRepository = accessionRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _patientRepository = patientRepository;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = await _accessionRepository.GetById(request.AccessionId, cancellationToken: cancellationToken);
            var isNewPatient = request.PatientId == null;

            if (isNewPatient)
            {
                if (request.PatientForCreationDto == null)
                    throw new ArgumentNullException(nameof(request.PatientForCreationDto));
                
                var newPatientToCreate = request.PatientForCreationDto.ToPatientForCreation();
                var newPatient = Patient.Create(newPatientToCreate);
                await _patientRepository.Add(newPatient, cancellationToken);
                accession.SetPatient(newPatient);
            }
            else
            {
                var existingPatient = await _patientRepository.GetById(request.PatientId!.Value, cancellationToken: cancellationToken);
                accession.SetPatient(existingPatient);
            }

            _accessionRepository.Update(accession);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}