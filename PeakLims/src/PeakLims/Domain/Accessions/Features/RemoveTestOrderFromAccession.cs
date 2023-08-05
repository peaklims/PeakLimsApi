namespace PeakLims.Domain.Accessions.Features;

using HeimGuard;
using MediatR;
using PeakLims.Domain;
using PeakLims.Services;
using Services;
using SharedKernel.Exceptions;
using TestOrders.Services;

public static class RemoveTestOrderFromAccession
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid AccessionId;
        public readonly Guid TestOrderId;

        public Command( Guid accessionId, Guid testOrderId)
        {
            AccessionId = accessionId;
            TestOrderId = testOrderId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
    {
        private readonly IAccessionRepository _accessionRepository;
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, IAccessionRepository accessionRepository, ITestOrderRepository testOrderRepository)
        {
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _accessionRepository = accessionRepository;
            _testOrderRepository = testOrderRepository;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanDeleteTestOrders);

            var accession = await _accessionRepository.GetWithTestOrderWithChildren(request.AccessionId, true, cancellationToken);
            var testOrderToRemove = await _testOrderRepository.GetById(request.TestOrderId, true, cancellationToken);
            accession.RemoveTestOrder(testOrderToRemove);
            await _unitOfWork.CommitChanges(cancellationToken);
            
            _testOrderRepository.CleanupOrphanedTestOrders();
            await _unitOfWork.CommitChanges(cancellationToken);
            return true;
        }
    }
}