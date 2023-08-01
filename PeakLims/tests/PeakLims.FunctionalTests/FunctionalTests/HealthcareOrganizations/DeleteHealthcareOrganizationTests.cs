namespace PeakLims.FunctionalTests.FunctionalTests.HealthcareOrganizations;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class DeleteHealthcareOrganizationTests : TestBase
{
    [Fact]
    public async Task delete_healthcareorganization_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakeHealthcareOrganization = new FakeHealthcareOrganizationBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeHealthcareOrganization);

        // Act
        var route = ApiRoutes.HealthcareOrganizations.Delete(fakeHealthcareOrganization.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task delete_healthcareorganization_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeHealthcareOrganization = new FakeHealthcareOrganizationBuilder().Build();

        // Act
        var route = ApiRoutes.HealthcareOrganizations.Delete(fakeHealthcareOrganization.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task delete_healthcareorganization_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeHealthcareOrganization = new FakeHealthcareOrganizationBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.HealthcareOrganizations.Delete(fakeHealthcareOrganization.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}