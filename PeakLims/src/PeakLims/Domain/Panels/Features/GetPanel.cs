namespace PeakLims.Domain.Panels.Features;

using PeakLims.Domain.Panels.Dtos;
using PeakLims.Domain.Panels.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class GetPanel
{
    public sealed class Query : IRequest<PanelDto>
    {
        public readonly Guid Id;

        public Query(Guid id)
        {
            Id = id;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PanelDto>
    {
        private readonly IPanelRepository _panelRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPanelRepository panelRepository, IHeimGuardClient heimGuard)
        {
            _panelRepository = panelRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PanelDto> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadPanels);

            var result = await _panelRepository.GetById(request.Id, cancellationToken: cancellationToken);
            return result.ToPanelDto();
        }
    }
}