namespace PeakLims.Domain.PatientRelationships.Features;

using Databases;
using Dtos;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit.Configuration;

public static class GetPatientRelationships
{
    public sealed record Query(Guid PatientId) : IRequest<List<PatientRelationshipDto>>;
    
    public sealed class Handler(PeakLimsDbContext dbContext) : IRequestHandler<Query, List<PatientRelationshipDto>>
    {
        public async Task<List<PatientRelationshipDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var patientRelationships = await dbContext.PatientRelationships
                .Where(x => x.FromPatientId == request.PatientId || x.ToPatientId == request.PatientId)
                .AsNoTracking()
                .Select(x => new PatientRelationshipDto()
                {
                    FromPatient = x.FromPatient.ToPatientRelationshipData(),
                    FromRelationship = x.FromRelationship.Value,
                    ToPatient = x.ToPatient.ToPatientRelationshipData(),
                    ToRelationship = x.ToRelationship.Value,
                    Notes = x.Notes,
                })
                .ToListAsync(cancellationToken);
            
            return GetDistinctRelationships(patientRelationships);
        }
        
        private static List<PatientRelationshipDto> GetDistinctRelationships(List<PatientRelationshipDto> patientRelationships)
        {
            var distinctList = new List<PatientRelationshipDto>();
            var relationshipMap = new Dictionary<(Guid fromId, Guid toId, string fromRel, string toRel), PatientRelationshipDto>();

            foreach (var rel in patientRelationships)
            {
                var key = (rel.FromPatient.Id, rel.ToPatient.Id, rel.FromRelationship, rel.ToRelationship);
                var reverseKey = (rel.ToPatient.Id, rel.FromPatient.Id, rel.ToRelationship, rel.FromRelationship);

                if (relationshipMap.TryGetValue(reverseKey, out var existing))
                {
                    existing.IsBidirectional = true;
                    continue;
                }

                relationshipMap[key] = rel;
                distinctList.Add(rel);
            }

            return distinctList;
        }
    }
}