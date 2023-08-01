namespace PeakLims.UnitTests.Domain.HealthcareOrganizationContacts;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.HealthcareOrganizationContacts.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class CreateHealthcareOrganizationContactTests
{
    private readonly Faker _faker;

    public CreateHealthcareOrganizationContactTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_healthcareOrganizationContact()
    {
        // Arrange
        var healthcareOrganizationContactToCreate = new FakeHealthcareOrganizationContactForCreation().Generate();
        
        // Act
        var fakeHealthcareOrganizationContact = HealthcareOrganizationContact.Create(healthcareOrganizationContactToCreate);

        // Assert
        fakeHealthcareOrganizationContact.Name.Should().Be(healthcareOrganizationContactToCreate.Name);
        fakeHealthcareOrganizationContact.Email.Should().Be(healthcareOrganizationContactToCreate.Email);
        fakeHealthcareOrganizationContact.Npi.Should().Be(healthcareOrganizationContactToCreate.Npi);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var healthcareOrganizationContactToCreate = new FakeHealthcareOrganizationContactForCreation().Generate();
        
        // Act
        var fakeHealthcareOrganizationContact = HealthcareOrganizationContact.Create(healthcareOrganizationContactToCreate);

        // Assert
        fakeHealthcareOrganizationContact.DomainEvents.Count.Should().Be(1);
        fakeHealthcareOrganizationContact.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(HealthcareOrganizationContactCreated));
    }
}