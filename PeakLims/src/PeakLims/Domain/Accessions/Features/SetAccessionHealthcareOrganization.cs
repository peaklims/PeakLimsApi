namespace PeakLims.Domain.Accessions.Features;

using Databases;
using Exceptions;
using HealthcareOrganizations.Services;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;

public static class SetAccessionHealthcareOrganization
{
    public sealed record Command(Guid AccessionId, Guid HealthcareOrganizationId) : IRequest<bool>;

    public sealed class Handler(
        IAccessionRepository accessionRepository,
        IUnitOfWork unitOfWork,
        PeakLimsDbContext dbContext, 
        IHeimGuardClient heimGuard,
        IHealthcareOrganizationRepository healthcareOrganizationRepository)
        : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = await dbContext.GetAccessionAggregate()
                .GetById(request.AccessionId, cancellationToken: cancellationToken);
            accession.MustBeFoundOrThrow();
            
            var healthcareOrganization = await healthcareOrganizationRepository.GetById(request.HealthcareOrganizationId, cancellationToken: cancellationToken);
            accession.SetHealthcareOrganization(healthcareOrganization);

            accessionRepository.Update(accession);
            return await unitOfWork.CommitChanges(cancellationToken) >= 1;
        }
    }
}