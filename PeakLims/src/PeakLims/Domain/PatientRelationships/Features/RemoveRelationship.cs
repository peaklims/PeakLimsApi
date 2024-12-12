namespace PeakLims.Domain.PatientRelationships.Features;

using Databases;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class RemoveRelationship
{
    public sealed record Command(Guid RelationshipId) : IRequest;
    
    public sealed class Handler(PeakLimsDbContext dbContext) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var relationship = await dbContext.PatientRelationships
                .Include(x => x.FromPatient)
                .Include(x => x.ToPatient)
                .GetById(request.RelationshipId, cancellationToken);

            relationship.FromPatient.RemoveRelative(relationship);
            dbContext.Update(relationship.FromPatient);
            dbContext.Update(relationship.ToPatient);
            // dbContext.PatientRelationships.Remove(relationship);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
