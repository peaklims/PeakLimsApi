namespace PeakLims.Domain.Tests.Features;

using PeakLims.Domain.Tests.Services;
using PeakLims.Domain.Tests;
using PeakLims.Domain.Tests.Dtos;
using PeakLims.Domain.Tests.Models;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddTest
{
    public sealed class Command : IRequest<TestDto>
    {
        public readonly TestForCreationDto TestToAdd;

        public Command(TestForCreationDto testToAdd)
        {
            TestToAdd = testToAdd;
        }
    }

    public sealed class Handler : IRequestHandler<Command, TestDto>
    {
        private readonly ITestRepository _testRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(ITestRepository testRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _testRepository = testRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task<TestDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddTests);

            var testToAdd = request.TestToAdd.ToTestForCreation();
            var test = Test.Create(testToAdd);

            await _testRepository.Add(test, cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);

            return test.ToTestDto();
        }
    }
}