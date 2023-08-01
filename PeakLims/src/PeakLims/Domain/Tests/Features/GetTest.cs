namespace PeakLims.Domain.Tests.Features;

using PeakLims.Domain.Tests.Dtos;
using PeakLims.Domain.Tests.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class GetTest
{
    public sealed class Query : IRequest<TestDto>
    {
        public readonly Guid Id;

        public Query(Guid id)
        {
            Id = id;
        }
    }

    public sealed class Handler : IRequestHandler<Query, TestDto>
    {
        private readonly ITestRepository _testRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(ITestRepository testRepository, IHeimGuardClient heimGuard)
        {
            _testRepository = testRepository;
            _heimGuard = heimGuard;
        }

        public async Task<TestDto> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadTests);

            var result = await _testRepository.GetById(request.Id, cancellationToken: cancellationToken);
            return result.ToTestDto();
        }
    }
}