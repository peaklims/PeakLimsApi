namespace PeakLims.UnitTests.Domain.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.Accessions.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class UpdateAccessionTests
{
    private readonly Faker _faker;

    public UpdateAccessionTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_accession()
    {
        // Arrange
        var fakeAccession = new FakeAccessionBuilder().Build();
        var updatedAccession = new FakeAccessionForUpdate().Generate();
        
        // Act
        fakeAccession.Update(updatedAccession);

        // Assert
        fakeAccession.AccessionNumber.Should().Be(updatedAccession.AccessionNumber);
        fakeAccession.Status.Should().Be(updatedAccession.Status);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakeAccession = new FakeAccessionBuilder().Build();
        var updatedAccession = new FakeAccessionForUpdate().Generate();
        fakeAccession.DomainEvents.Clear();
        
        // Act
        fakeAccession.Update(updatedAccession);

        // Assert
        fakeAccession.DomainEvents.Count.Should().Be(1);
        fakeAccession.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(AccessionUpdated));
    }
}