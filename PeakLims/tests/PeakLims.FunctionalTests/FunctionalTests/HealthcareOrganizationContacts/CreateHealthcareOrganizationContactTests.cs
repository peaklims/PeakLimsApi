namespace PeakLims.FunctionalTests.FunctionalTests.HealthcareOrganizationContacts;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class CreateHealthcareOrganizationContactTests : TestBase
{
    [Fact]
    public async Task create_healthcareorganizationcontact_returns_created_using_valid_dto_and_valid_auth_credentials()
    {
        // Arrange
        var fakeHealthcareOrganizationContact = new FakeHealthcareOrganizationContactForCreationDto().Generate();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);

        // Act
        var route = ApiRoutes.HealthcareOrganizationContacts.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeHealthcareOrganizationContact);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }
            
    [Fact]
    public async Task create_healthcareorganizationcontact_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeHealthcareOrganizationContact = new FakeHealthcareOrganizationContactForCreationDto { }.Generate();

        // Act
        var route = ApiRoutes.HealthcareOrganizationContacts.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeHealthcareOrganizationContact);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task create_healthcareorganizationcontact_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeHealthcareOrganizationContact = new FakeHealthcareOrganizationContactForCreationDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.HealthcareOrganizationContacts.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeHealthcareOrganizationContact);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}