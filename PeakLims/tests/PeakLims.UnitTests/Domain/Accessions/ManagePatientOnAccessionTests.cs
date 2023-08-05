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

public class ManagePatientOnAccessionTests
{
    private readonly Faker _faker;

    public ManagePatientOnAccessionTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void can_manage_patient()
    {
        // Arrange
        var accession = Accession.Create();
        var patient = new FakePatientBuilder().Build();
        
        // Act - Add
        accession.SetPatient(patient);

        // Assert - Add
        accession.Patient.Should().BeEquivalentTo(patient);
        accession.Patient.Id.Should().Be(patient.Id);
        
        // Act - Remove
        accession.RemovePatient();

        // Assert - Remove
        accession.Patient.Should().BeNull();
    }

    [Fact]
    public void can_not_update_patient_when_processing_accession()
    {
        // Arrange
        var accession = new FakeAccessionBuilder()
            .WithSetupForValidReadyForTestingTransition()
            .Build();
        accession.DomainEvents.Clear();
        accession.SetStatusToReadyForTesting();

        var anotherPatient = new FakePatientBuilder().Build();
        
        // Act
        var actAdd = () => accession.SetPatient(anotherPatient);
        var actRemove = () => accession.RemovePatient();

        // Assert
        actAdd.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage($"This accession is processing. The patient can not be modified.");
        actRemove.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage($"This accession is processing. The patient can not be modified.");
    }

    [Fact]
    public void must_use_valid_patient()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();
        
        // Act
        var actAdd = () => accession.SetPatient(null);

        // Assert
        actAdd.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage($"Invalid Patient.");
    }
}