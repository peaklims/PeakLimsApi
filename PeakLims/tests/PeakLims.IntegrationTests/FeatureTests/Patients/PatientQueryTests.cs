namespace PeakLims.IntegrationTests.FeatureTests.Patients;

using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.Domain.Patients.Features;
using SharedKernel.Exceptions;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class PatientQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_patient_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePatientOne = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fakePatientOne);

        // Act
        var query = new GetPatient.Query(fakePatientOne.Id);
        var patient = await testingServiceScope.SendAsync(query);

        // Assert
        patient.FirstName.Should().Be(fakePatientOne.FirstName);
        patient.LastName.Should().Be(fakePatientOne.LastName);
        patient.DateOfBirth.Should().Be(fakePatientOne.Lifespan.DateOfBirth);
        patient.Age.Should().Be(fakePatientOne.Lifespan.KnownAge);
        patient.Sex.Should().Be(fakePatientOne.Sex.Value);
        patient.Race.Should().Be(fakePatientOne.Race.Value);
        patient.Ethnicity.Should().Be(fakePatientOne.Ethnicity.Value);
        patient.InternalId.Should().Be(fakePatientOne.InternalId);
    }

    [Fact]
    public async Task get_patient_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetPatient.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadPatients);

        // Act
        var command = new GetPatient.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}