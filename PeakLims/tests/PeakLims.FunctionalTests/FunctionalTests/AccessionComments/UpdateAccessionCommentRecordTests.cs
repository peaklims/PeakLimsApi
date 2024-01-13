namespace PeakLims.FunctionalTests.FunctionalTests.AccessionComments;

using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class UpdateAccessionCommentRecordTests : TestBase
{
    [Fact]
    public async Task put_accessioncomment_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeAccessionComment = new FakeAccessionCommentBuilder().Build();
        var updatedAccessionCommentDto = new FakeAccessionCommentForUpdateDto { }.Generate();

        // Act
        var route = ApiRoutes.AccessionComments.Put(fakeAccessionComment.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedAccessionCommentDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task put_accessioncomment_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeAccessionComment = new FakeAccessionCommentBuilder().Build();
        var updatedAccessionCommentDto = new FakeAccessionCommentForUpdateDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.AccessionComments.Put(fakeAccessionComment.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedAccessionCommentDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}