namespace PeakLims.UnitTests.Domain.PeakOrganizations;

using PeakLims.SharedTestHelpers.Fakes.PeakOrganization;
using PeakLims.Domain.PeakOrganizations;
using PeakLims.Domain.PeakOrganizations.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using ValidationException = PeakLims.Exceptions.ValidationException;

public class UpdatePeakOrganizationTests
{
    private readonly Faker _faker;

    public UpdatePeakOrganizationTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_peakOrganization()
    {
        // Arrange
        var peakOrganization = new FakePeakOrganizationBuilder().Build();
        var updatedPeakOrganization = new FakePeakOrganizationForUpdate().Generate();
        
        // Act
        peakOrganization.Update(updatedPeakOrganization);

        // Assert
        peakOrganization.Name.Should().Be(updatedPeakOrganization.Name);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var peakOrganization = new FakePeakOrganizationBuilder().Build();
        var updatedPeakOrganization = new FakePeakOrganizationForUpdate().Generate();
        peakOrganization.DomainEvents.Clear();
        
        // Act
        peakOrganization.Update(updatedPeakOrganization);

        // Assert
        peakOrganization.DomainEvents.Count.Should().Be(1);
        peakOrganization.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PeakOrganizationUpdated));
    }
}