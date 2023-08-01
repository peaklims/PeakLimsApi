namespace PeakLims.IntegrationTests.FeatureTests.Patients;

using PeakLims.Domain.Patients.Dtos;
using PeakLims.SharedTestHelpers.Fakes.Patient;
using SharedKernel.Exceptions;
using PeakLims.Domain.Patients.Features;
using FluentAssertions;
using Domain;
using Xunit;
using System.Threading.Tasks;

public class PatientListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_patient_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePatientOne = new FakePatientBuilder().Build();
        var fakePatientTwo = new FakePatientBuilder().Build();
        var queryParameters = new PatientParametersDto();

        await testingServiceScope.InsertAsync(fakePatientOne, fakePatientTwo);

        // Act
        var query = new GetPatientList.Query(queryParameters);
        var patients = await testingServiceScope.SendAsync(query);

        // Assert
        patients.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadPatients);
        var queryParameters = new PatientParametersDto();

        // Act
        var command = new GetPatientList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}