namespace PeakLims.FunctionalTests.FunctionalTests.AccessionContacts;

using PeakLims.SharedTestHelpers.Fakes.AccessionContact;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class DeleteAccessionContactTests : TestBase
{
    [Fact]
    public async Task delete_accessioncontact_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakeAccessionContact = new FakeAccessionContactBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeAccessionContact);

        // Act
        var route = ApiRoutes.AccessionContacts.Delete(fakeAccessionContact.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task delete_accessioncontact_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeAccessionContact = new FakeAccessionContactBuilder().Build();

        // Act
        var route = ApiRoutes.AccessionContacts.Delete(fakeAccessionContact.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task delete_accessioncontact_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeAccessionContact = new FakeAccessionContactBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.AccessionContacts.Delete(fakeAccessionContact.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}