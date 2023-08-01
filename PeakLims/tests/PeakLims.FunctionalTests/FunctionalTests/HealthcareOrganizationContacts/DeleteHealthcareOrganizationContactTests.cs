namespace PeakLims.FunctionalTests.FunctionalTests.HealthcareOrganizationContacts;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class DeleteHealthcareOrganizationContactTests : TestBase
{
    [Fact]
    public async Task delete_healthcareorganizationcontact_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakeHealthcareOrganizationContact = new FakeHealthcareOrganizationContactBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeHealthcareOrganizationContact);

        // Act
        var route = ApiRoutes.HealthcareOrganizationContacts.Delete(fakeHealthcareOrganizationContact.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task delete_healthcareorganizationcontact_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeHealthcareOrganizationContact = new FakeHealthcareOrganizationContactBuilder().Build();

        // Act
        var route = ApiRoutes.HealthcareOrganizationContacts.Delete(fakeHealthcareOrganizationContact.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task delete_healthcareorganizationcontact_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeHealthcareOrganizationContact = new FakeHealthcareOrganizationContactBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.HealthcareOrganizationContacts.Delete(fakeHealthcareOrganizationContact.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}