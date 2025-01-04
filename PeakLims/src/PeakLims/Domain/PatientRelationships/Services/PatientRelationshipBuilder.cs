namespace PeakLims.Domain.PatientRelationships.Services;

using Patients;
using RelationshipTypes;

public static class PatientRelationshipBuilder
{
    public static IIs For(Patient fromPatient)
        => new BuilderImpl(fromPatient);

    private sealed class BuilderImpl : IIs, IAs, IWhoIs, IWithNotes
    {
        private readonly Patient _fromPatient;
        private RelationshipType _fromRelationshipType;
        private Patient _toPatient;
        private RelationshipType _toRelationshipType;
        private string _notes = string.Empty;

        public BuilderImpl(Patient fromPatient)
        {
            _fromPatient = fromPatient;
        }

        public IAs As(RelationshipType fromRelationshipType)
        {
            _fromRelationshipType = fromRelationshipType;
            return this;
        }

        public IWhoIs To(Patient toPatient)
        {
            _toPatient = toPatient;
            return this;
        }

        public IWithNotes WhoIs(RelationshipType toRelationshipType)
        {
            _toRelationshipType = toRelationshipType;
            return this;
        }

        public IWithNotes WithNotes(string? notes)
        {
            _notes = notes;
            return this;
        }

        public PatientRelationship Build()
        {
            var newRelationship = new PatientRelationship().Create(
                _fromPatient,
                _toPatient,
                _fromRelationshipType,
                _toRelationshipType,
                _notes
            );
            return newRelationship;
        }
    }

    public interface IIs
    {
        IAs As(RelationshipType fromRelationshipType);
    }

    public interface IAs
    {
        IWhoIs To(Patient toPatient);
    }

    public interface IWhoIs
    {
        IWithNotes WhoIs(RelationshipType toRelationshipType);
    }

    public interface IWithNotes
    {
        IWithNotes WithNotes(string? notes);
        PatientRelationship Build();
    }
}