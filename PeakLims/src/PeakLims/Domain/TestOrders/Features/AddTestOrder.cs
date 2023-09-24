namespace PeakLims.Domain.TestOrders.Features;

using Accessions;
using Accessions.Services;
using PeakLims.Domain.TestOrders.Services;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Panels.Services;
using Tests.Services;

public static class AddTestOrder
{
    public sealed record Command(Guid AccessionId, Guid? TestId, Guid? PanelId) : IRequest;

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;
        private readonly ITestRepository _testRepository;
        private readonly IPanelRepository _panelRepository;
        private readonly IAccessionRepository _accessionRepository;

        public Handler(ITestOrderRepository testOrderRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, ITestRepository testRepository, IPanelRepository panelRepository, IAccessionRepository accessionRepository)
        {
            _testOrderRepository = testOrderRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _testRepository = testRepository;
            _panelRepository = panelRepository;
            _accessionRepository = accessionRepository;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddTestOrders);

            if(request.PanelId != null && request.TestId != null)
                throw new ValidationException("Cannot add both a test and a panel to a test order.");
            
            var accession = await _accessionRepository.Query()
                .Include(x => x.TestOrders)
                .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken);
            
            if (accession == null)
            {
                throw new NotFoundException(nameof(Accession), request.AccessionId);
            }
            
            if(request.TestId.HasValue)
            {
                var test = await _testRepository.GetById(request.TestId.Value, true, cancellationToken);
                var testOrder = accession.AddTest(test);
                await _testOrderRepository.Add(testOrder, cancellationToken);
            }
            if(request.PanelId.HasValue)
            {
                var panel = await _panelRepository.GetById(request.PanelId.Value, true, cancellationToken);
                var testOrders = accession.AddPanel(panel);
                await _testOrderRepository.AddRange(testOrders, cancellationToken);
            }
            
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}