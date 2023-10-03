namespace PeakLims.FunctionalTests.FunctionalTests.AccessionAttachments;

using PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using System.Net;
using System.Threading.Tasks;

public class GetAccessionAttachmentTests : TestBase
{
    [Fact]
    public async Task get_accessionattachment_returns_success_when_entity_exists_using_valid_auth_credentials()
    {
        // Arrange
        var accessionAttachment = new FakeAccessionAttachmentBuilder().Build();

        var callingUser = await AddNewSuperAdmin();
        FactoryClient.AddAuth(callingUser.Identifier);
        await InsertAsync(accessionAttachment);

        // Act
        var route = ApiRoutes.AccessionAttachments.GetRecord(accessionAttachment.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
            
    [Fact]
    public async Task get_accessionattachment_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var accessionAttachment = new FakeAccessionAttachmentBuilder().Build();

        // Act
        var route = ApiRoutes.AccessionAttachments.GetRecord(accessionAttachment.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task get_accessionattachment_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var accessionAttachment = new FakeAccessionAttachmentBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.AccessionAttachments.GetRecord(accessionAttachment.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}