namespace PeakLims.Domain.Tests.Features;

using Exceptions;
using PeakLims.Domain.Tests;
using PeakLims.Domain.Tests.Dtos;
using PeakLims.Domain.Tests.Services;
using PeakLims.Services;
using PeakLims.Domain.Tests.Models;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateTest
{
    public sealed class Command : IRequest
    {
        public readonly Guid Id;
        public readonly TestForUpdateDto UpdatedTestData;

        public Command(Guid id, TestForUpdateDto updatedTestData)
        {
            Id = id;
            UpdatedTestData = updatedTestData;
        }
    }

    public sealed class Handler : IRequestHandler<Command>
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

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var testToUpdate = await _testRepository.GetById(request.Id, cancellationToken: cancellationToken);
            var testToAdd = request.UpdatedTestData.ToTestForUpdate();
            testToUpdate.Update(testToAdd);

            _testRepository.Update(testToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}