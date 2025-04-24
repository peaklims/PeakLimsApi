namespace PeakLims.Domain.PatientRelationships.Features;

using Databases;
using Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Patients;
using RelationshipTypes;

public static class GetSuggestedMatchingRole
{
    public sealed record Command(GetSuggestedMatchingRoleRequestDto GetSuggestedMatchingRoleRequestDto) : IRequest<SuggestedMatchingRoleDto>;
    
    public sealed class Handler(PeakLimsDbContext dbContext) : IRequestHandler<Command, SuggestedMatchingRoleDto>
    {
        public async Task<SuggestedMatchingRoleDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var toPatient = await dbContext.Patients
                .GetById(request.GetSuggestedMatchingRoleRequestDto.ToPatientId, cancellationToken: cancellationToken);
            var relationshipType = RelationshipType.Of(request.GetSuggestedMatchingRoleRequestDto.Relationship);

            return new SuggestedMatchingRoleDto(relationshipType.GetSuggestedMatchingRole(toPatient.Sex).Value);
        }
    }
}