namespace PeakLims.UnitTests.Domain.PeakOrganizations;

using PeakLims.SharedTestHelpers.Fakes.PeakOrganization;
using PeakLims.Domain.PeakOrganizations;
using PeakLims.Domain.PeakOrganizations.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using ValidationException = PeakLims.Exceptions.ValidationException;

public class CreatePeakOrganizationTests
{
    private readonly Faker _faker;

    public CreatePeakOrganizationTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_peakOrganization()
    {
        // Arrange
        var peakOrganizationToCreate = new FakePeakOrganizationForCreation().Generate();
        
        // Act
        var peakOrganization = PeakOrganization.Create(peakOrganizationToCreate);

        // Assert
        peakOrganization.Name.Should().Be(peakOrganizationToCreate.Name);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var peakOrganizationToCreate = new FakePeakOrganizationForCreation().Generate();
        
        // Act
        var peakOrganization = PeakOrganization.Create(peakOrganizationToCreate);

        // Assert
        peakOrganization.DomainEvents.Count.Should().Be(1);
        peakOrganization.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PeakOrganizationCreated));
    }
}