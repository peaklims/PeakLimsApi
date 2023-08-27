namespace PeakLims.FunctionalTests.FunctionalTests.AccessionContacts;

using PeakLims.SharedTestHelpers.Fakes.AccessionContact;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class CreateAccessionContactTests : TestBase
{
    [Fact]
    public async Task create_accessioncontact_returns_created_using_valid_dto_and_valid_auth_credentials()
    {
        // Arrange
        var fakeAccessionContact = new FakeAccessionContactForCreationDto().Generate();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);

        // Act
        var route = ApiRoutes.AccessionContacts.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeAccessionContact);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }
            
    [Fact]
    public async Task create_accessioncontact_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeAccessionContact = new FakeAccessionContactForCreationDto { }.Generate();

        // Act
        var route = ApiRoutes.AccessionContacts.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeAccessionContact);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task create_accessioncontact_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeAccessionContact = new FakeAccessionContactForCreationDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.AccessionContacts.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeAccessionContact);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}