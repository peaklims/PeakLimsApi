namespace PeakLims.Domain.Panels.Features;

using Exceptions;
using PeakLims.Domain.Panels.Services;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;

public static class DeletePanel
{
    public sealed class Command : IRequest
    {
        public readonly Guid Id;

        public Command(Guid id)
        {
            Id = id;
        }
    }

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IPanelRepository _panelRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPanelRepository panelRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _panelRepository = panelRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var recordToDelete = await _panelRepository.GetById(request.Id, cancellationToken: cancellationToken);
            _panelRepository.Remove(recordToDelete);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}