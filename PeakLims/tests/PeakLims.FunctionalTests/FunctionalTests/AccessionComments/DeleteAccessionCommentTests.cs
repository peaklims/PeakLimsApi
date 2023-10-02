namespace PeakLims.FunctionalTests.FunctionalTests.AccessionComments;

using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class DeleteAccessionCommentTests : TestBase
{
    [Fact]
    public async Task delete_accessioncomment_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakeAccessionComment = new FakeAccessionCommentBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeAccessionComment);

        // Act
        var route = ApiRoutes.AccessionComments.Delete(fakeAccessionComment.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task delete_accessioncomment_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeAccessionComment = new FakeAccessionCommentBuilder().Build();

        // Act
        var route = ApiRoutes.AccessionComments.Delete(fakeAccessionComment.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task delete_accessioncomment_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeAccessionComment = new FakeAccessionCommentBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.AccessionComments.Delete(fakeAccessionComment.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}