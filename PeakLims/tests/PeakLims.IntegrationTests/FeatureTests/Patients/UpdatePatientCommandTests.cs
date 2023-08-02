namespace PeakLims.IntegrationTests.FeatureTests.Patients;

using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.Domain.Patients.Dtos;
using SharedKernel.Exceptions;
using PeakLims.Domain.Patients.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class UpdatePatientCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_patient_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePatientOne = new FakePatientBuilder().Build();
        var updatedPatientDto = new FakePatientForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakePatientOne);

        var patient = await testingServiceScope.ExecuteDbContextAsync(db => db.Patients
            .FirstOrDefaultAsync(p => p.Id == fakePatientOne.Id));

        // Act
        var command = new UpdatePatient.Command(patient.Id, updatedPatientDto);
        await testingServiceScope.SendAsync(command);
        var updatedPatient = await testingServiceScope.ExecuteDbContextAsync(db => db.Patients.FirstOrDefaultAsync(p => p.Id == patient.Id));

        // Assert
        updatedPatient.FirstName.Should().Be(updatedPatientDto.FirstName);
        updatedPatient.LastName.Should().Be(updatedPatientDto.LastName);
        updatedPatient.Lifespan.DateOfBirth.Should().Be(updatedPatientDto.DateOfBirth);
        updatedPatient.Sex.Value.Should().Be(updatedPatientDto.Sex);
        updatedPatient.Race.Value.Should().Be(updatedPatientDto.Race);
        updatedPatient.Ethnicity.Value.Should().Be(updatedPatientDto.Ethnicity);
        updatedPatient.InternalId.Should().NotBeNull();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdatePatients);
        var fakePatientOne = new FakePatientForUpdateDto();

        // Act
        var command = new UpdatePatient.Command(Guid.NewGuid(), fakePatientOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}