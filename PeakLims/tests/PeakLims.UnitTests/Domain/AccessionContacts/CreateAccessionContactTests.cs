namespace PeakLims.UnitTests.Domain.AccessionContacts;

using PeakLims.SharedTestHelpers.Fakes.AccessionContact;
using PeakLims.Domain.AccessionContacts;
using PeakLims.Domain.AccessionContacts.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class CreateAccessionContactTests
{
    private readonly Faker _faker;

    public CreateAccessionContactTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_accessionContact()
    {
        // Arrange
        var accessionContactToCreate = new FakeAccessionContactForCreation().Generate();
        
        // Act
        var fakeAccessionContact = AccessionContact.Create(accessionContactToCreate);

        // Assert
        fakeAccessionContact.TargetType.Should().Be(accessionContactToCreate.TargetType);
        fakeAccessionContact.TargetValue.Should().Be(accessionContactToCreate.TargetValue);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var accessionContactToCreate = new FakeAccessionContactForCreation().Generate();
        
        // Act
        var fakeAccessionContact = AccessionContact.Create(accessionContactToCreate);

        // Assert
        fakeAccessionContact.DomainEvents.Count.Should().Be(1);
        fakeAccessionContact.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(AccessionContactCreated));
    }
}