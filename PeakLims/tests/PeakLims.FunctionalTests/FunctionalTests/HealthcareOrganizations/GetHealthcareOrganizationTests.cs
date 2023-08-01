namespace PeakLims.FunctionalTests.FunctionalTests.HealthcareOrganizations;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class GetHealthcareOrganizationTests : TestBase
{
    [Fact]
    public async Task get_healthcareorganization_returns_success_when_entity_exists_using_valid_auth_credentials()
    {
        // Arrange
        var fakeHealthcareOrganization = new FakeHealthcareOrganizationBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeHealthcareOrganization);

        // Act
        var route = ApiRoutes.HealthcareOrganizations.GetRecord(fakeHealthcareOrganization.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
            
    [Fact]
    public async Task get_healthcareorganization_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeHealthcareOrganization = new FakeHealthcareOrganizationBuilder().Build();

        // Act
        var route = ApiRoutes.HealthcareOrganizations.GetRecord(fakeHealthcareOrganization.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task get_healthcareorganization_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeHealthcareOrganization = new FakeHealthcareOrganizationBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.HealthcareOrganizations.GetRecord(fakeHealthcareOrganization.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}