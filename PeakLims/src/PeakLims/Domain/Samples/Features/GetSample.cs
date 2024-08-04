namespace PeakLims.Domain.Samples.Features;

using Exceptions;
using PeakLims.Domain.Samples.Dtos;
using PeakLims.Domain.Samples.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class GetSample
{
    public sealed class Query : IRequest<SampleDto>
    {
        public readonly Guid Id;

        public Query(Guid id)
        {
            Id = id;
        }
    }

    public sealed class Handler(ISampleRepository sampleRepository, IHeimGuardClient heimGuard)
        : IRequestHandler<Query, SampleDto>
    {
        public async Task<SampleDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await sampleRepository.GetById(request.Id, cancellationToken: cancellationToken);
            return result.ToSampleDto();
        }
    }
}