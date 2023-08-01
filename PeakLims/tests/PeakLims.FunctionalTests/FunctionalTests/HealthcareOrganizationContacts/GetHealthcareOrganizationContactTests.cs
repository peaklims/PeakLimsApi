namespace PeakLims.FunctionalTests.FunctionalTests.HealthcareOrganizationContacts;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class GetHealthcareOrganizationContactTests : TestBase
{
    [Fact]
    public async Task get_healthcareorganizationcontact_returns_success_when_entity_exists_using_valid_auth_credentials()
    {
        // Arrange
        var fakeHealthcareOrganizationContact = new FakeHealthcareOrganizationContactBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeHealthcareOrganizationContact);

        // Act
        var route = ApiRoutes.HealthcareOrganizationContacts.GetRecord(fakeHealthcareOrganizationContact.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
            
    [Fact]
    public async Task get_healthcareorganizationcontact_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeHealthcareOrganizationContact = new FakeHealthcareOrganizationContactBuilder().Build();

        // Act
        var route = ApiRoutes.HealthcareOrganizationContacts.GetRecord(fakeHealthcareOrganizationContact.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task get_healthcareorganizationcontact_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeHealthcareOrganizationContact = new FakeHealthcareOrganizationContactBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.HealthcareOrganizationContacts.GetRecord(fakeHealthcareOrganizationContact.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}