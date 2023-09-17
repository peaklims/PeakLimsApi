namespace PeakLims.Domain.Samples.Features;

using Databases;
using PeakLims.Domain.Samples.Dtos;
using PeakLims.Domain.Samples.Services;
using PeakLims.Wrappers;
using SharedKernel.Exceptions;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Patients.Services;
using QueryKit;
using QueryKit.Configuration;

public static class GetSamplesByPatient
{
    public sealed record Query(Guid PatientId) : IRequest<List<SampleDto>>;

    public sealed class Handler : IRequestHandler<Query, List<SampleDto>>
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(ISampleRepository sampleRepository, IHeimGuardClient heimGuard, IPatientRepository patientRepository)
        {
            _sampleRepository = sampleRepository;
            _heimGuard = heimGuard;
            _patientRepository = patientRepository;
        }

        public async Task<List<SampleDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadSamples);
            
            await _patientRepository.GetById(request.PatientId, false, cancellationToken);
            
            return await _sampleRepository.Query()
                .Include(x => x.Container)
                .Where(x => x.Patient.Id == request.PatientId)
                .OrderByDescending(x => x.SampleNumber)
                .AsNoTracking()
                .ToSampleDtoQueryable()
                .ToListAsync(cancellationToken);
        }
    }
}