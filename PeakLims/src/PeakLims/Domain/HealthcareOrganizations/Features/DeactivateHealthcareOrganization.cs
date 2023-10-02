namespace PeakLims.Domain.HealthcareOrganizations.Features;

using Exceptions;
using HeimGuard;
using MediatR;
using PeakLims.Domain;
using PeakLims.Domain.HealthcareOrganizations.Services;
using PeakLims.Services;

public static class DeactivateHealthcareOrganization
{
    public sealed record Command(Guid OrgId) : IRequest;

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IHealthcareOrganizationRepository _orgRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IHealthcareOrganizationRepository orgRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _orgRepository = orgRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanDeactivateHealthcareOrganizations);

            var orgToUpdate = await _orgRepository.GetById(request.OrgId, cancellationToken: cancellationToken);
            orgToUpdate.Deactivate();
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}