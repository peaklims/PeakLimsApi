namespace PeakLims.Domain.Accessions.Features;

using HealthcareOrganizations.Services;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.Accessions.Dtos;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Patients.Services;

public static class AddAccession
{
    public sealed class Command : IRequest<AccessionDto>
    {
        public Guid? PatientId { get; }
        public Guid? HealthcareOrganizationId { get; }
        
        public Command(Guid? patientId = null, Guid? healthcareOrganizationId = null)
        {
            PatientId = patientId;
            HealthcareOrganizationId = healthcareOrganizationId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, AccessionDto>
    {
        private readonly IAccessionRepository _accessionRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IHealthcareOrganizationRepository _healthcareOrganizationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionRepository accessionRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, IPatientRepository patientRepository, IHealthcareOrganizationRepository healthcareOrganizationRepository)
        {
            _accessionRepository = accessionRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _patientRepository = patientRepository;
            _healthcareOrganizationRepository = healthcareOrganizationRepository;
        }

        public async Task<AccessionDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddAccessions);
            
            var accession = Accession.Create();
            await _accessionRepository.Add(accession, cancellationToken);

            var hasPatientId = Guid.TryParse(request.PatientId.ToString(), out var patientId);
            if (hasPatientId)
            {
                var patient = await _patientRepository.GetById(patientId, true, cancellationToken);
                accession.SetPatient(patient);
            }
            var hasHealthcareOrganizationId = Guid.TryParse(request.HealthcareOrganizationId.ToString(), out var healthcareOrganizationId);
            if (hasHealthcareOrganizationId)
            {
                var healthcareOrganization = await _healthcareOrganizationRepository.GetById(healthcareOrganizationId, true, cancellationToken);
                accession.SetHealthcareOrganization(healthcareOrganization);
            }

            await _unitOfWork.CommitChanges(cancellationToken);

            return accession.ToAccessionDto();
        }
    }
}