namespace PeakLims.UnitTests.Domain.Patients;

using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class UpdatePatientTests
{
    private readonly Faker _faker;

    public UpdatePatientTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_patient()
    {
        // Arrange
        var fakePatient = new FakePatientBuilder().Build();
        var updatedPatient = new FakePatientForUpdate().Generate();
        
        // Act
        fakePatient.Update(updatedPatient);

        // Assert
        fakePatient.FirstName.Should().Be(updatedPatient.FirstName);
        fakePatient.LastName.Should().Be(updatedPatient.LastName);
        fakePatient.Lifespan.DateOfBirth.Should().Be(updatedPatient.DateOfBirth);
        fakePatient.Sex.Value.Should().Be(updatedPatient.Sex);
        fakePatient.Race.Value.Should().Be(updatedPatient.Race);
        fakePatient.Ethnicity.Value.Should().Be(updatedPatient.Ethnicity);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakePatient = new FakePatientBuilder().Build();
        var updatedPatient = new FakePatientForUpdate().Generate();
        fakePatient.DomainEvents.Clear();
        
        // Act
        fakePatient.Update(updatedPatient);

        // Assert
        fakePatient.DomainEvents.Count.Should().Be(1);
        fakePatient.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(PatientUpdated));
    }
}