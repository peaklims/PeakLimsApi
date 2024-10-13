namespace PeakLims.Domain.PeakOrganizations.Features;

using PeakLims.Domain.PeakOrganizations;
using PeakLims.Domain.PeakOrganizations.Dtos;
using PeakLims.Databases;
using PeakLims.Services;
using PeakLims.Domain.PeakOrganizations.Models;
using PeakLims.Exceptions;
using Mappings;
using MediatR;

public static class UpdatePeakOrganization
{
    public sealed record Command(Guid PeakOrganizationId, PeakOrganizationForUpdateDto UpdatedPeakOrganizationData) : IRequest;

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var peakOrganizationToUpdate = await dbContext.PeakOrganizations.GetById(request.PeakOrganizationId, cancellationToken);
            var peakOrganizationToAdd = request.UpdatedPeakOrganizationData.ToPeakOrganizationForUpdate();
            peakOrganizationToUpdate.Update(peakOrganizationToAdd);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}