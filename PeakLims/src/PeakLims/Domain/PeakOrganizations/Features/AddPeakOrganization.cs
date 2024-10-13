namespace PeakLims.Domain.PeakOrganizations.Features;

using PeakLims.Databases;
using PeakLims.Domain.PeakOrganizations;
using PeakLims.Domain.PeakOrganizations.Dtos;
using PeakLims.Domain.PeakOrganizations.Models;
using PeakLims.Services;
using PeakLims.Exceptions;
using Mappings;
using MediatR;

public static class AddPeakOrganization
{
    public sealed record Command(PeakOrganizationForCreationDto PeakOrganizationToAdd) : IRequest<PeakOrganizationDto>;

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Command, PeakOrganizationDto>
    {
        public async Task<PeakOrganizationDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var peakOrganizationToAdd = request.PeakOrganizationToAdd.ToPeakOrganizationForCreation();
            var peakOrganization = PeakOrganization.Create(peakOrganizationToAdd);

            await dbContext.PeakOrganizations.AddAsync(peakOrganization, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return peakOrganization.ToPeakOrganizationDto();
        }
    }
}