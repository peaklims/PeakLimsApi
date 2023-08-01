namespace PeakLims.UnitTests.Domain.HealthcareOrganizations;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using PeakLims.Domain.HealthcareOrganizations;
using PeakLims.Domain.HealthcareOrganizations.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class CreateHealthcareOrganizationTests
{
    private readonly Faker _faker;

    public CreateHealthcareOrganizationTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_healthcareOrganization()
    {
        // Arrange
        var healthcareOrganizationToCreate = new FakeHealthcareOrganizationForCreation().Generate();
        
        // Act
        var fakeHealthcareOrganization = HealthcareOrganization.Create(healthcareOrganizationToCreate);

        // Assert
        fakeHealthcareOrganization.Name.Should().Be(healthcareOrganizationToCreate.Name);
        fakeHealthcareOrganization.Email.Should().Be(healthcareOrganizationToCreate.Email);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var healthcareOrganizationToCreate = new FakeHealthcareOrganizationForCreation().Generate();
        
        // Act
        var fakeHealthcareOrganization = HealthcareOrganization.Create(healthcareOrganizationToCreate);

        // Assert
        fakeHealthcareOrganization.DomainEvents.Count.Should().Be(1);
        fakeHealthcareOrganization.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(HealthcareOrganizationCreated));
    }
}