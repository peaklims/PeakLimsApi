namespace PeakLims.Domain.Samples.Features;

using Containers;
using Containers.Services;
using Exceptions;
using PeakLims.Domain.Samples.Services;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Dtos;
using PeakLims.Domain.Samples.Models;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Patients.Services;

public static class AddSample
{
    public sealed record Command(SampleForCreationDto SampleToAdd) : IRequest<SampleDto>;

    public sealed class Handler : IRequestHandler<Command, SampleDto>
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;
        private readonly IContainerRepository _containerRepository;
        private readonly IPatientRepository _patientRepository;

        public Handler(ISampleRepository sampleRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, IContainerRepository containerRepository, IPatientRepository patientRepository)
        {
            _sampleRepository = sampleRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _containerRepository = containerRepository;
            _patientRepository = patientRepository;
        }

        public async Task<SampleDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddSamples);

            var patient = await _patientRepository.GetById(request.SampleToAdd.PatientId, true, cancellationToken);

            var sampleToAdd = request.SampleToAdd.ToSampleForCreation();
            var sample = Sample.Create(sampleToAdd);
            patient.AddSample(sample);

            if (request.SampleToAdd.ContainerId != null)
            {
                var container = await _containerRepository.GetById(request.SampleToAdd.ContainerId.Value, true, cancellationToken);
                sample.SetContainer(container);
            }

            await _sampleRepository.Add(sample, cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);

            return sample.ToSampleDto();
        }
    }
}