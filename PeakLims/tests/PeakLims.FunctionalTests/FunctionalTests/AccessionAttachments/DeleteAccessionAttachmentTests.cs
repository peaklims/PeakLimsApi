namespace PeakLims.FunctionalTests.FunctionalTests.AccessionAttachments;

using PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using System.Net;
using System.Threading.Tasks;

public class DeleteAccessionAttachmentTests : TestBase
{
    [Fact]
    public async Task delete_accessionattachment_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var accessionAttachment = new FakeAccessionAttachmentBuilder().Build();

        var callingUser = await AddNewSuperAdmin();
        FactoryClient.AddAuth(callingUser.Identifier);
        await InsertAsync(accessionAttachment);

        // Act
        var route = ApiRoutes.AccessionAttachments.Delete(accessionAttachment.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task delete_accessionattachment_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var accessionAttachment = new FakeAccessionAttachmentBuilder().Build();

        // Act
        var route = ApiRoutes.AccessionAttachments.Delete(accessionAttachment.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task delete_accessionattachment_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var accessionAttachment = new FakeAccessionAttachmentBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.AccessionAttachments.Delete(accessionAttachment.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}