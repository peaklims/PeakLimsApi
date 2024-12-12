namespace PeakLims.Domain.PatientRelationships.Features;

using Databases;
using Dtos;
using MediatR;
using Patients;

public static class AddRelationship
{
    public sealed record Command(AddPatientRelationshipDto AddPatientRelationshipDto) : IRequest;
    
    public sealed class Handler(PeakLimsDbContext dbContext) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var fromPatient = await dbContext.Patients
                .GetById(request.AddPatientRelationshipDto.FromPatientId, cancellationToken: cancellationToken);
            var toPatient = await dbContext.Patients
                .GetById(request.AddPatientRelationshipDto.ToPatientId, cancellationToken: cancellationToken);

            var relationships = fromPatient.AddRelative(request.AddPatientRelationshipDto.FromRelationship,
                toPatient,
                request.AddPatientRelationshipDto.ToRelationship,
                request.AddPatientRelationshipDto.Notes,
                request.AddPatientRelationshipDto.IsConfirmedBidirectional);
            
            dbContext.PatientRelationships.AddRange(relationships);
            
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}