namespace PeakLims.UnitTests.Domain.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.Accessions.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using PeakLims.Domain.AccessionStatuses;
using Xunit;

public class CreateAccessionTests
{
    private readonly Faker _faker;

    public CreateAccessionTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_accession()
    {
        // Arrange
        // Act
        var fakeAccession = Accession.Create();

        // Assert
        fakeAccession.Status.Should().Be(AccessionStatus.Draft());
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        // Act
        var fakeAccession = Accession.Create();

        // Assert
        fakeAccession.DomainEvents.Count.Should().Be(1);
        fakeAccession.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(AccessionCreated));
    }
}