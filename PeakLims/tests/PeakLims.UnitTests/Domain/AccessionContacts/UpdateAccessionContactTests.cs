namespace PeakLims.UnitTests.Domain.AccessionContacts;

using PeakLims.SharedTestHelpers.Fakes.AccessionContact;
using PeakLims.Domain.AccessionContacts;
using PeakLims.Domain.AccessionContacts.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class UpdateAccessionContactTests
{
    private readonly Faker _faker;

    public UpdateAccessionContactTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_accessionContact()
    {
        // Arrange
        var fakeAccessionContact = new FakeAccessionContactBuilder().Build();
        var updatedAccessionContact = new FakeAccessionContactForUpdate().Generate();
        
        // Act
        fakeAccessionContact.Update(updatedAccessionContact);

        // Assert
        fakeAccessionContact.TargetType.Should().Be(updatedAccessionContact.TargetType);
        fakeAccessionContact.TargetValue.Should().Be(updatedAccessionContact.TargetValue);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakeAccessionContact = new FakeAccessionContactBuilder().Build();
        var updatedAccessionContact = new FakeAccessionContactForUpdate().Generate();
        fakeAccessionContact.DomainEvents.Clear();
        
        // Act
        fakeAccessionContact.Update(updatedAccessionContact);

        // Assert
        fakeAccessionContact.DomainEvents.Count.Should().Be(1);
        fakeAccessionContact.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(AccessionContactUpdated));
    }
}