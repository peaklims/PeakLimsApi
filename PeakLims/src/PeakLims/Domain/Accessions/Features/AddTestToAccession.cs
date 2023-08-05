namespace PeakLims.Domain.Accessions.Features;

using HeimGuard;
using MediatR;
using PeakLims.Domain;
using PeakLims.Services;
using Services;
using SharedKernel.Exceptions;
using TestOrders.Services;
using Tests.Services;

public static class AddTestToAccession
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid AccessionId;
        public readonly Guid TestId;

        public Command( Guid accessionId, Guid testId)
        {
            AccessionId = accessionId;
            TestId = testId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
    {
        private readonly ITestRepository _testRepository;
        private readonly IAccessionRepository _accessionRepository;
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, ITestRepository testRepository, IAccessionRepository accessionRepository, ITestOrderRepository testOrderRepository)
        {
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _testRepository = testRepository;
            _accessionRepository = accessionRepository;
            _testOrderRepository = testOrderRepository;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddTestToAccessions);

            var accession = await _accessionRepository.GetWithTestOrderWithChildren(request.AccessionId, true, cancellationToken);
            var testToAdd = await _testRepository.GetById(request.TestId, true, cancellationToken);
            var existingTestOrders = accession.TestOrders.ToList();
            accession.AddTest(testToAdd);

            await _testOrderRepository.AddRange(accession.TestOrders.Except(existingTestOrders), cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);
            return true;
        }
    }
}