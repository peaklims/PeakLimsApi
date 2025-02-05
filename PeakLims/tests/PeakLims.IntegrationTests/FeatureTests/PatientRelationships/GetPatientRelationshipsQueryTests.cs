namespace PeakLims.IntegrationTests.FeatureTests.PatientRelationships;

using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.Domain.PatientRelationships.Features;
using PeakLims.Domain.PatientRelationships.Services;
using PeakLims.Domain.RelationshipTypes;
using FluentAssertions;
using System.Threading.Tasks;
using Bogus;

public class GetPatientRelationshipsQueryTests : TestBase
{
    private readonly Faker _faker = new();
    
    [Fact]
    public async Task can_get_patient_relationships()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fromPatient = new FakePatientBuilder().Build();
        var toPatient = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fromPatient, toPatient);
        var toRelationshipType = _faker.PickRandom(RelationshipType.ListNames());
        var fromRelationshipType = _faker.PickRandom(RelationshipType.ListNames());
        
        var relationship = PatientRelationshipBuilder
            .For(fromPatient)
            .As(new RelationshipType(toRelationshipType))
            .To(toPatient)
            .WhoIs(new RelationshipType(fromRelationshipType))
            .WithNotes(_faker.Lorem.Sentence())
            .Build();

        await testingServiceScope.InsertAsync(relationship);

        // Act
        var query = new GetPatientRelationships.Query(fromPatient.Id);
        var relationships = await testingServiceScope.SendAsync(query);

        // Assert
        relationships.Count.Should().Be(1);
        relationships[0].FromPatient.Id.Should().Be(fromPatient.Id);
        relationships[0].ToPatient.Id.Should().Be(toPatient.Id);
        relationships[0].FromRelationship.Should().Be(toRelationshipType);
        relationships[0].ToRelationship.Should().Be(fromRelationshipType);
        relationships[0].Notes.Should().Be(relationship.Notes);
    }

    [Fact]
    public async Task can_get_bidirectional_relationships()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fromPatient = new FakePatientBuilder().Build();
        var toPatient = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fromPatient, toPatient);
        var toRelationshipType = _faker.PickRandom(RelationshipType.ListNames());
        var fromRelationshipType = _faker.PickRandom(RelationshipType.ListNames());
        
        var relationship1 = PatientRelationshipBuilder
            .For(fromPatient)
            .As(new RelationshipType(fromRelationshipType))
            .To(toPatient)
            .WhoIs(new RelationshipType(toRelationshipType))
            .WithNotes(_faker.Lorem.Sentence())
            .Build();

        var relationship2 = relationship1.BuildReverseRelationship();

        await testingServiceScope.InsertAsync(relationship1, relationship2);

        // Act
        var query = new GetPatientRelationships.Query(fromPatient.Id);
        var relationships = await testingServiceScope.SendAsync(query);

        // Assert
        relationships.Count.Should().Be(1);
        relationships[0].IsBidirectional.Should().BeTrue();
        relationships[0].FromRelationship.Should().Be(fromRelationshipType);
        relationships[0].ToRelationship.Should().Be(toRelationshipType);
        relationships[0].Notes.Should().Be(relationship1.Notes);
    }
}
