namespace PeakLims.IntegrationTests.FeatureTests.Patients;

using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.Domain.Patients.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class DeletePatientCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_patient_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePatientOne = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fakePatientOne);
        var patient = await testingServiceScope.ExecuteDbContextAsync(db => db.Patients
            .FirstOrDefaultAsync(p => p.Id == fakePatientOne.Id));

        // Act
        var command = new DeletePatient.Command(patient.Id);
        await testingServiceScope.SendAsync(command);
        var patientResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.Patients.CountAsync(p => p.Id == patient.Id));

        // Assert
        patientResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_patient_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeletePatient.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_patient_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePatientOne = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fakePatientOne);
        var patient = await testingServiceScope.ExecuteDbContextAsync(db => db.Patients
            .FirstOrDefaultAsync(p => p.Id == fakePatientOne.Id));

        // Act
        var command = new DeletePatient.Command(patient.Id);
        await testingServiceScope.SendAsync(command);
        var deletedPatient = await testingServiceScope.ExecuteDbContextAsync(db => db.Patients
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == patient.Id));

        // Assert
        deletedPatient?.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeletePatients);

        // Act
        var command = new DeletePatient.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}