namespace PeakLims.Domain.PatientRelationships;

using Patients;
using RelationshipTypes;

public class PatientRelationship : BaseEntity
{
    public Guid FromPatientId { get; private set;}
    public Patient FromPatient { get; private set;}
    public Guid ToPatientId { get; private set; }
    public Patient ToPatient { get; private set; }
    public RelationshipType FromRelationship { get; private set; }
    public RelationshipType ToRelationship { get; private set; }
    public string? Notes { get; private set; }

    public PatientRelationship Create(Patient fromPatient, Patient toPatient, RelationshipType fromRelationship, 
        RelationshipType toRelationship, string? notes)
    {
        var newPatientRelationship = new PatientRelationship();
        newPatientRelationship.FromPatientId = fromPatient.Id;
        newPatientRelationship.FromPatient = fromPatient;
        newPatientRelationship.ToPatientId = toPatient.Id;
        newPatientRelationship.ToPatient = toPatient;
        newPatientRelationship.FromRelationship = fromRelationship;
        newPatientRelationship.ToRelationship = toRelationship;
        newPatientRelationship.Notes = notes;

        return newPatientRelationship;
    }
    
}