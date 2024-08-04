namespace PeakLims.Domain.Samples.Features;

using Containers.Services;
using Exceptions;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Dtos;
using PeakLims.Domain.Samples.Services;
using PeakLims.Services;
using PeakLims.Domain.Samples.Models;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class DisposeSample
{
    public sealed record Command(Guid SampleId) : IRequest;

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
            var sample = await _sampleRepository.GetById(request.SampleId, cancellationToken: cancellationToken);
            sample.Dispose();

            _sampleRepository.Update(sample);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}