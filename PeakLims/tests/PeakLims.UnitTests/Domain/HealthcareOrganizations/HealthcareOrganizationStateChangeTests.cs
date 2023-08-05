namespace PeakLims.UnitTests.Domain.HealthcareOrganizations;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.HealthcareOrganizations.DomainEvents;
using PeakLims.Domain.HealthcareOrganizationStatuses;
using SharedTestHelpers.Fakes.HealthcareOrganization;
using Xunit;

public class HealthcareOrganizationStateChangeTests
{
    private readonly Faker _faker;

    public HealthcareOrganizationStateChangeTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_activate_test()
    {
        // Arrange
        var fakeHealthcareOrganization = new FakeHealthcareOrganizationBuilder().Build();
        fakeHealthcareOrganization.Deactivate();
        fakeHealthcareOrganization.DomainEvents.Clear();
        
        // Act
        fakeHealthcareOrganization.Activate();

        // Assert
        fakeHealthcareOrganization.Status.Should().Be(HealthcareOrganizationStatus.Active());
        fakeHealthcareOrganization.DomainEvents.Count.Should().Be(1);
        fakeHealthcareOrganization.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(HealthcareOrganizationUpdated));
    }
    
    [Fact]
    public void can_deactivate_test()
    {
        // Arrange
        var fakeHealthcareOrganization = new FakeHealthcareOrganizationBuilder().Build();
        fakeHealthcareOrganization.Activate();
        fakeHealthcareOrganization.DomainEvents.Clear();
        
        // Act
        fakeHealthcareOrganization.Deactivate();

        // Assert
        fakeHealthcareOrganization.Status.Should().Be(HealthcareOrganizationStatus.Inactive());
        fakeHealthcareOrganization.DomainEvents.Count.Should().Be(1);
        fakeHealthcareOrganization.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(HealthcareOrganizationUpdated));
    }
}