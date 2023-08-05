namespace PeakLims.UnitTests.Domain.Accessions;

using Bogus;
using FluentAssertions;
using Moq;
using PeakLims.Domain.Accessions;
using PeakLims.Services;
using PeakLims.SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.Container;
using SharedTestHelpers.Fakes.HealthcareOrganization;
using SharedTestHelpers.Fakes.Patient;
using SharedTestHelpers.Fakes.Sample;
using Xunit;

public class ManageHealthcareOrganizationOnAccessionTests
{
    private readonly Faker _faker;

    public ManageHealthcareOrganizationOnAccessionTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void can_manage_healthcareOrganization()
    {
        // Arrange
        var accession = Accession.Create();
        var healthcareOrganization = new FakeHealthcareOrganizationBuilder().Build();
        
        // Act - add
        accession.SetHealthcareOrganization(healthcareOrganization);

        // Assert - Add
        accession.HealthcareOrganization.Should().BeEquivalentTo(healthcareOrganization);
        accession.HealthcareOrganization.Id.Should().Be(healthcareOrganization.Id);
        
        // Act - remove
        accession.RemoveHealthcareOrganization();

        // Assert - Remove
        accession.HealthcareOrganization.Should().BeNull();
    }

    [Fact]
    public void can_not_add_inactive_org()
    {
        // Arrange
        var accession = Accession.Create();
        var healthcareOrganization = new FakeHealthcareOrganizationBuilder().Build();
        healthcareOrganization.Deactivate();
        
        // Act - add
        var act = () => accession.SetHealthcareOrganization(healthcareOrganization);

        // Assert - Add
        act.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage("Only active organizations can be set on an accession.");
    }

    [Fact]
    public void can_not_update_org_when_processing_accession()
    {
        // Arrange
        var accession = new FakeAccessionBuilder()
            .WithSetupForValidReadyForTestingTransition()
            .Build();
        accession.SetStatusToReadyForTesting();

        var anotherOrg = new FakeHealthcareOrganizationBuilder().Build();
        
        // Act
        var actAdd = () => accession.SetHealthcareOrganization(anotherOrg);
        var actRemove = () => accession.RemoveHealthcareOrganization();

        // Assert
        actAdd.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage($"This accession is processing. The organization can not be modified.");
        actRemove.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage($"This accession is processing. The organization can not be modified.");
    }

    [Fact]
    public void must_use_valid_org()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();
        
        // Act
        var actAdd = () => accession.SetHealthcareOrganization(null);

        // Assert
        actAdd.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage($"Invalid Healthcare Organization.");
    }
}