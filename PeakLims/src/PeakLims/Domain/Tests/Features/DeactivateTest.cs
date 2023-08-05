namespace PeakLims.Domain.Tests.Features;

using HeimGuard;
using MediatR;
using PeakLims.Domain;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;
using Services;
using SharedKernel.Exceptions;

public static class DeactivateTest
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid Id;

        public Command(Guid accessionId)
        {
            Id = accessionId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
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

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanDeactivateTests);

            var testToUpdate = await _testRepository.GetById(request.Id, cancellationToken: cancellationToken);
            testToUpdate.Deactivate();
            return await _unitOfWork.CommitChanges(cancellationToken) >= 1;
        }
    }
}