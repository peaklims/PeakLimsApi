namespace PeakLims.UnitTests.Domain.HealthcareOrganizationContacts;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.HealthcareOrganizationContacts.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class UpdateHealthcareOrganizationContactTests
{
    [Fact]
    public void can_update_healthcareOrganizationContact()
    {
        // Arrange
        var fakeHealthcareOrganizationContact = new FakeHealthcareOrganizationContactBuilder().Build();
        var updatedHealthcareOrganizationContact = new FakeHealthcareOrganizationContactForUpdate().Generate();
        
        // Act
        fakeHealthcareOrganizationContact.Update(updatedHealthcareOrganizationContact);

        // Assert
        fakeHealthcareOrganizationContact.FirstName.Should().Be(updatedHealthcareOrganizationContact.FirstName);
        fakeHealthcareOrganizationContact.LastName.Should().Be(updatedHealthcareOrganizationContact.LastName);
        fakeHealthcareOrganizationContact.Title.Should().Be(updatedHealthcareOrganizationContact.Title);
        fakeHealthcareOrganizationContact.Email.Should().Be(updatedHealthcareOrganizationContact.Email);
        fakeHealthcareOrganizationContact.Npi.Value.Should().Be(updatedHealthcareOrganizationContact.Npi);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakeHealthcareOrganizationContact = new FakeHealthcareOrganizationContactBuilder().Build();
        var updatedHealthcareOrganizationContact = new FakeHealthcareOrganizationContactForUpdate().Generate();
        fakeHealthcareOrganizationContact.DomainEvents.Clear();
        
        // Act
        fakeHealthcareOrganizationContact.Update(updatedHealthcareOrganizationContact);

        // Assert
        fakeHealthcareOrganizationContact.DomainEvents.Count.Should().Be(1);
        fakeHealthcareOrganizationContact.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(HealthcareOrganizationContactUpdated));
    }
}