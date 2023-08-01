namespace PeakLims.FunctionalTests.FunctionalTests.Patients;

using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class UpdatePatientRecordTests : TestBase
{
    [Fact]
    public async Task put_patient_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakePatient = new FakePatientBuilder().Build();
        var updatedPatientDto = new FakePatientForUpdateDto().Generate();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakePatient);

        // Act
        var route = ApiRoutes.Patients.Put(fakePatient.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedPatientDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task put_patient_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakePatient = new FakePatientBuilder().Build();
        var updatedPatientDto = new FakePatientForUpdateDto { }.Generate();

        // Act
        var route = ApiRoutes.Patients.Put(fakePatient.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedPatientDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task put_patient_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakePatient = new FakePatientBuilder().Build();
        var updatedPatientDto = new FakePatientForUpdateDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Patients.Put(fakePatient.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedPatientDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}