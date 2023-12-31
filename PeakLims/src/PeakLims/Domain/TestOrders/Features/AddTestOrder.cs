namespace PeakLims.Domain.TestOrders.Features;

using Accessions;
using Accessions.Services;
using Exceptions;
using PeakLims.Domain.TestOrders.Services;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Panels.Services;
using Tests.Services;

public static class AddTestOrder
{
    public sealed record Command(Guid AccessionId, Guid TestId) : IRequest;

    public sealed class Handler(ITestOrderRepository testOrderRepository, IUnitOfWork unitOfWork,
            IHeimGuardClient heimGuard, ITestRepository testRepository, IPanelRepository panelRepository,
            IAccessionRepository accessionRepository)
        : IRequestHandler<Command>
    {
        private readonly IPanelRepository _panelRepository = panelRepository;

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddTestOrders);
            
            var accession = await accessionRepository.Query()
                .Include(x => x.TestOrders)
                .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken);
            
            if (accession == null)
            {
                throw new NotFoundException(nameof(Accession), request.AccessionId);
            }
            
            var test = await testRepository.GetById(request.TestId, true, cancellationToken);
            var testOrder = accession.AddTest(test);
            await testOrderRepository.Add(testOrder, cancellationToken);
            
            
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}