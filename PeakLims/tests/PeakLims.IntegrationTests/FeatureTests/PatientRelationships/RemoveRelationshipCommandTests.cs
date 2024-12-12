namespace PeakLims.IntegrationTests.FeatureTests.PatientRelationships;

using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.Domain.PatientRelationships.Features;
using PeakLims.Domain.PatientRelationships.Services;
using PeakLims.Domain.RelationshipTypes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Bogus;

public class RemoveRelationshipCommandTests : TestBase
{
    private readonly Faker _faker = new();
    
    [Fact]
    public async Task can_remove_relationship()
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

        var command = new RemoveRelationship.Command(relationship.Id);

        // Act
        await testingServiceScope.SendAsync(command);
        var relationshipExists = await testingServiceScope
            .ExecuteDbContextAsync(db => db.PatientRelationships
                .AnyAsync(x => x.Id == relationship.Id));

        // Assert
        relationshipExists.Should().BeFalse();
    }

    [Fact]
    public async Task can_remove_bidirectional_relationships()
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

        var relationship2 = PatientRelationshipBuilder
            .For(toPatient)
            .As(new RelationshipType(toRelationshipType))
            .To(fromPatient)
            .WhoIs(new RelationshipType(fromRelationshipType))
            .WithNotes(_faker.Lorem.Sentence())
            .Build();

        await testingServiceScope.InsertAsync(relationship1, relationship2);

        var command = new RemoveRelationship.Command(relationship1.Id);

        // Act
        await testingServiceScope.SendAsync(command);
        var relationship1Exists = await testingServiceScope
            .ExecuteDbContextAsync(db => db.PatientRelationships
                .AnyAsync(x => x.Id == relationship1.Id));
        var relationship2Exists = await testingServiceScope
            .ExecuteDbContextAsync(db => db.PatientRelationships
                .AnyAsync(x => x.Id == relationship2.Id));

        // Assert
        relationship1Exists.Should().BeFalse();
        relationship2Exists.Should().BeFalse();
    }
}
