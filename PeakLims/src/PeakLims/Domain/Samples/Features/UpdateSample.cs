namespace PeakLims.Domain.Samples.Features;

using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Dtos;
using PeakLims.Domain.Samples.Services;
using PeakLims.Services;
using PeakLims.Domain.Samples.Models;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateSample
{
    public sealed class Command : IRequest
    {
        public readonly Guid Id;
        public readonly SampleForUpdateDto UpdatedSampleData;

        public Command(Guid id, SampleForUpdateDto updatedSampleData)
        {
            Id = id;
            UpdatedSampleData = updatedSampleData;
        }
    }

    public sealed class Handler : IRequestHandler<Command>
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

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateSamples);

            var sampleToUpdate = await _sampleRepository.GetById(request.Id, cancellationToken: cancellationToken);
            var sampleToAdd = request.UpdatedSampleData.ToSampleForUpdate();
            sampleToUpdate.Update(sampleToAdd);

            _sampleRepository.Update(sampleToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}