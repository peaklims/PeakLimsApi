namespace PeakLims.Domain.Accessions.Features;

using Exceptions;
using HealthcareOrganizations.Services;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.Accessions.Dtos;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Patients.Services;

public static class AddAccession
{
    public sealed record Command(Guid? PatientId = null, Guid? HealthcareOrganizationId = null) : IRequest<AccessionDto>;

    public sealed class Handler(
        IAccessionRepository accessionRepository,
        IUnitOfWork unitOfWork,
        IPatientRepository patientRepository,
        ICurrentUserService currentUserService,
        IHealthcareOrganizationRepository healthcareOrganizationRepository)
        : IRequestHandler<Command, AccessionDto>
    {
        public async Task<AccessionDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = Accession.Create(currentUserService.GetOrganizationId());
            await accessionRepository.Add(accession, cancellationToken);

            var hasPatientId = Guid.TryParse(request.PatientId.ToString(), out var patientId);
            if (hasPatientId)
            {
                var patient = await patientRepository.GetById(patientId, true, cancellationToken);
                accession.SetPatient(patient);
            }
            var hasHealthcareOrganizationId = Guid.TryParse(request.HealthcareOrganizationId.ToString(), out var healthcareOrganizationId);
            if (hasHealthcareOrganizationId)
            {
                var healthcareOrganization = await healthcareOrganizationRepository.GetById(healthcareOrganizationId, true, cancellationToken);
                accession.SetHealthcareOrganization(healthcareOrganization);
            }

            await unitOfWork.CommitChanges(cancellationToken);

            return accession.ToAccessionDto();
        }
    }
}