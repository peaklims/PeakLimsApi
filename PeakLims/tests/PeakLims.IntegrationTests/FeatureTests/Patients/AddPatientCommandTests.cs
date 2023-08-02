namespace PeakLims.IntegrationTests.FeatureTests.Patients;

using PeakLims.SharedTestHelpers.Fakes.Patient;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using PeakLims.Domain.Patients.Features;
using SharedKernel.Exceptions;

public class AddPatientCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_patient_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePatientOne = new FakePatientForCreationDto().Generate();

        // Act
        var command = new AddPatient.Command(fakePatientOne);
        var patientReturned = await testingServiceScope.SendAsync(command);
        var patientCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.Patients
            .FirstOrDefaultAsync(p => p.Id == patientReturned.Id));

        // Assert
        patientReturned.FirstName.Should().Be(fakePatientOne.FirstName);
        patientReturned.LastName.Should().Be(fakePatientOne.LastName);
        patientReturned.DateOfBirth.Should().Be(fakePatientOne.DateOfBirth);
        patientReturned.Sex.Should().Be(fakePatientOne.Sex);
        patientReturned.Race.Should().Be(fakePatientOne.Race);
        patientReturned.Ethnicity.Should().Be(fakePatientOne.Ethnicity);
        patientReturned.InternalId.Should().NotBeNull();

        patientCreated.FirstName.Should().Be(fakePatientOne.FirstName);
        patientCreated.LastName.Should().Be(fakePatientOne.LastName);
        patientCreated.Lifespan.DateOfBirth.Should().Be(fakePatientOne.DateOfBirth);
        patientCreated.Sex.Value.Should().Be(fakePatientOne.Sex);
        patientCreated.Race.Value.Should().Be(fakePatientOne.Race);
        patientCreated.Ethnicity.Value.Should().Be(fakePatientOne.Ethnicity);
        patientCreated.InternalId.Should().NotBeNull();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddPatients);
        var fakePatientOne = new FakePatientForCreationDto();

        // Act
        var command = new AddPatient.Command(fakePatientOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}