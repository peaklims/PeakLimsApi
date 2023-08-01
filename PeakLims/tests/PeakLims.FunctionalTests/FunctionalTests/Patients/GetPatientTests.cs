namespace PeakLims.FunctionalTests.FunctionalTests.Patients;

using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class GetPatientTests : TestBase
{
    [Fact]
    public async Task get_patient_returns_success_when_entity_exists_using_valid_auth_credentials()
    {
        // Arrange
        var fakePatient = new FakePatientBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakePatient);

        // Act
        var route = ApiRoutes.Patients.GetRecord(fakePatient.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
            
    [Fact]
    public async Task get_patient_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakePatient = new FakePatientBuilder().Build();

        // Act
        var route = ApiRoutes.Patients.GetRecord(fakePatient.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task get_patient_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakePatient = new FakePatientBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Patients.GetRecord(fakePatient.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}