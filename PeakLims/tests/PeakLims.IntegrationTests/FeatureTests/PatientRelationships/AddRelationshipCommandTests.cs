namespace PeakLims.IntegrationTests.FeatureTests.PatientRelationships;

using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.Domain.PatientRelationships.Features;
using PeakLims.Domain.PatientRelationships.Dtos;
using PeakLims.Domain.RelationshipTypes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Bogus;

public class AddRelationshipCommandTests : TestBase
{
    private readonly Faker _faker = new();
    
    [Fact]
    public async Task can_add_new_relationship_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fromPatient = new FakePatientBuilder().Build();
        var toPatient = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fromPatient, toPatient);
        var toRelationshipType = _faker.PickRandom(RelationshipType.ListNames());
        var fromRelationshipType = _faker.PickRandom(RelationshipType.ListNames());

        var relationshipDto = new AddPatientRelationshipDto()
        {
            FromPatientId = fromPatient.Id,
            ToPatientId = toPatient.Id,
            FromRelationship = fromRelationshipType,
            ToRelationship = toRelationshipType,
            Notes = _faker.Lorem.Sentence(),
            IsConfirmedBidirectional = true
        };

        // Act
        var command = new AddRelationship.Command(relationshipDto);
        await testingServiceScope.SendAsync(command);

        // Assert
        var relationship = await testingServiceScope.ExecuteDbContextAsync(db => db.PatientRelationships
            .FirstOrDefaultAsync(r => r.FromPatientId == fromPatient.Id && r.ToPatientId == toPatient.Id));
        
        relationship.Should().NotBeNull();
        relationship.FromPatientId.Should().Be(fromPatient.Id);
        relationship.ToPatientId.Should().Be(toPatient.Id);
        relationship.FromRelationship.Value.Should().Be(fromRelationshipType);
        relationship.ToRelationship.Value.Should().Be(toRelationshipType);
        relationship.Notes.Should().Be(relationshipDto.Notes);
    }
}
