namespace PeakLims.UnitTests.Domain.HealthcareOrganizations;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using PeakLims.Domain.HealthcareOrganizations;
using PeakLims.Domain.HealthcareOrganizations.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class UpdateHealthcareOrganizationTests
{
    private readonly Faker _faker;

    public UpdateHealthcareOrganizationTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_healthcareOrganization()
    {
        // Arrange
        var fakeHealthcareOrganization = new FakeHealthcareOrganizationBuilder().Build();
        var updatedHealthcareOrganization = new FakeHealthcareOrganizationForUpdate().Generate();
        
        // Act
        fakeHealthcareOrganization.Update(updatedHealthcareOrganization);

        // Assert
        fakeHealthcareOrganization.Name.Should().Be(updatedHealthcareOrganization.Name);
        fakeHealthcareOrganization.Email.Should().Be(updatedHealthcareOrganization.Email);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakeHealthcareOrganization = new FakeHealthcareOrganizationBuilder().Build();
        var updatedHealthcareOrganization = new FakeHealthcareOrganizationForUpdate().Generate();
        fakeHealthcareOrganization.DomainEvents.Clear();
        
        // Act
        fakeHealthcareOrganization.Update(updatedHealthcareOrganization);

        // Assert
        fakeHealthcareOrganization.DomainEvents.Count.Should().Be(1);
        fakeHealthcareOrganization.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(HealthcareOrganizationUpdated));
    }
}