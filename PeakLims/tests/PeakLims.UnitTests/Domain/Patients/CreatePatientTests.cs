namespace PeakLims.UnitTests.Domain.Patients;

using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class CreatePatientTests
{
    private readonly Faker _faker;

    public CreatePatientTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_patient()
    {
        // Arrange
        var patientToCreate = new FakePatientForCreation().Generate();
        
        // Act
        var fakePatient = Patient.Create(patientToCreate);

        // Assert
        fakePatient.FirstName.Should().Be(patientToCreate.FirstName);
        fakePatient.LastName.Should().Be(patientToCreate.LastName);
        fakePatient.DateOfBirth.Should().Be(patientToCreate.DateOfBirth);
        fakePatient.Age.Should().Be(patientToCreate.Age);
        fakePatient.Sex.Should().Be(patientToCreate.Sex);
        fakePatient.Race.Should().Be(patientToCreate.Race);
        fakePatient.Ethnicity.Should().Be(patientToCreate.Ethnicity);
        fakePatient.InternalId.Should().Be(patientToCreate.InternalId);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var patientToCreate = new FakePatientForCreation().Generate();
        
        // Act
        var fakePatient = Patient.Create(patientToCreate);

        // Assert
        fakePatient.DomainEvents.Count.Should().Be(1);
        fakePatient.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PatientCreated));
    }
}