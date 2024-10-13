namespace PeakLims.Domain.Tests.Features;

using Exceptions;
using PeakLims.Domain.Tests.Services;
using PeakLims.Domain.Tests;
using PeakLims.Domain.Tests.Dtos;
using PeakLims.Domain.Tests.Models;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddTest
{
    public sealed record Command(TestForCreationDto TestToAdd) : IRequest<TestDto>;

    public sealed class Handler(ITestRepository testRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        : IRequestHandler<Command, TestDto>
    {
        public async Task<TestDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var testToAdd = request.TestToAdd.ToTestForCreation(currentUserService.GetOrganizationId());
            var test = Test.Create(testToAdd);

            await testRepository.Add(test, cancellationToken);
            await unitOfWork.CommitChanges(cancellationToken);

            return test.ToTestDto();
        }
    }
}