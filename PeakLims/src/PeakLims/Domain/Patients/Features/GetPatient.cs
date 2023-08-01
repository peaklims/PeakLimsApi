namespace PeakLims.Domain.Patients.Features;

using PeakLims.Domain.Patients.Dtos;
using PeakLims.Domain.Patients.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class GetPatient
{
    public sealed class Query : IRequest<PatientDto>
    {
        public readonly Guid Id;

        public Query(Guid id)
        {
            Id = id;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PatientDto>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPatientRepository patientRepository, IHeimGuardClient heimGuard)
        {
            _patientRepository = patientRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PatientDto> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadPatients);

            var result = await _patientRepository.GetById(request.Id, cancellationToken: cancellationToken);
            return result.ToPatientDto();
        }
    }
}