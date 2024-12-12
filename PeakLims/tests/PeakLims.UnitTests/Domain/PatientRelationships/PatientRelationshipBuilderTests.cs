namespace PeakLims.UnitTests.Domain.PatientRelationships;

using AutoBogus;
using Bogus;
using PeakLims.Domain.PatientRelationships.Services;
using PeakLims.Domain.Patients;
using PeakLims.Domain.RelationshipTypes;
using SharedTestHelpers.Fakes.Patient;

public class PatientRelationshipBuilderTests
{
    [Fact]
    public void can_build_relationship_without_notes()
    {
        var fromPatient = new FakePatientBuilder().Build();
        var toPatient = new FakePatientBuilder().Build();
        var fromRel = RelationshipType.Son();
        var toRel = RelationshipType.Father();

        var result = PatientRelationshipBuilder
            .For(fromPatient)
            .As(fromRel)
            .To(toPatient)
            .WhoIs(toRel)
            .Build();

        result.Should().NotBeNull();
        result.FromPatientId.Should().Be(fromPatient.Id);
        result.ToPatientId.Should().Be(toPatient.Id);
        result.FromRelationship.Should().Be(fromRel);
        result.ToRelationship.Should().Be(toRel);
        result.Notes.Should().BeNullOrEmpty();
    }

    [Fact]
    public void can_build_relationship_with_notes()
    {
        var faker = new Faker();
        var fromPatient = new FakePatientBuilder().Build();
        var toPatient = new FakePatientBuilder().Build();
        var fromRel = RelationshipType.Son();
        var toRel = RelationshipType.Father();
        var notes = faker.Lorem.Sentence();

        var result = PatientRelationshipBuilder
            .For(fromPatient)
            .As(fromRel)
            .To(toPatient)
            .WhoIs(toRel)
            .WithNotes(notes)
            .Build();

        result.Should().NotBeNull();
        result.FromPatientId.Should().Be(fromPatient.Id);
        result.ToPatientId.Should().Be(toPatient.Id);
        result.FromRelationship.Should().Be(fromRel);
        result.ToRelationship.Should().Be(toRel);
        result.Notes.Should().Be(notes);
    }
}
