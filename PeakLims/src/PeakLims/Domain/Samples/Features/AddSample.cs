namespace PeakLims.Domain.Samples.Features;

using PeakLims.Domain.Samples.Services;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Dtos;
using PeakLims.Domain.Samples.Models;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddSample
{
    public sealed class Command : IRequest<SampleDto>
    {
        public readonly SampleForCreationDto SampleToAdd;

        public Command(SampleForCreationDto sampleToAdd)
        {
            SampleToAdd = sampleToAdd;
        }
    }

    public sealed class Handler : IRequestHandler<Command, SampleDto>
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(ISampleRepository sampleRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _sampleRepository = sampleRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task<SampleDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddSamples);

            var sampleToAdd = request.SampleToAdd.ToSampleForCreation();
            var sample = Sample.Create(sampleToAdd);

            await _sampleRepository.Add(sample, cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);

            return sample.ToSampleDto();
        }
    }
}