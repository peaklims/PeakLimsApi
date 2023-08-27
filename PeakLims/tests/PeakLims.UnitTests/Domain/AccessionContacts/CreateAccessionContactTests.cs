namespace PeakLims.UnitTests.Domain.AccessionContacts;

using PeakLims.SharedTestHelpers.Fakes.AccessionContact;
using PeakLims.Domain.AccessionContacts;
using PeakLims.Domain.AccessionContacts.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using SharedTestHelpers.Fakes.HealthcareOrganizationContact;
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
        var orgContact = new FakeHealthcareOrganizationContactBuilder().Build();
        
        // Act
        var accessionContact = AccessionContact.Create(orgContact);

        // Assert
        accessionContact.TargetType.Should().Be(TargetTypeEnum.Email.Name);
        accessionContact.TargetValue.Should().Be(orgContact.Email);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var accessionContactToCreate = new FakeAccessionContactForCreation().Generate();
        var orgContact = new FakeHealthcareOrganizationContactBuilder().Build();
        
        // Act
        var fakeAccessionContact = AccessionContact.Create(orgContact);

        // Assert
        fakeAccessionContact.DomainEvents.Count.Should().Be(1);
        fakeAccessionContact.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(AccessionContactCreated));
    }
}