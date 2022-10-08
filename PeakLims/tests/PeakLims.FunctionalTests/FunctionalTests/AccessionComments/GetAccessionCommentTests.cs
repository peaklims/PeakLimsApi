namespace PeakLims.FunctionalTests.FunctionalTests.AccessionComments;

using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Threading.Tasks;

public class GetAccessionCommentTests : TestBase
{
    [Test]
    public async Task get_accessioncomment_returns_success_when_entity_exists_using_valid_auth_credentials()
    {
        // Arrange
        var fakeAccessionOne = FakeAccession.Generate(new FakeAccessionForCreationDto().Generate());
        await InsertAsync(fakeAccessionOne);

        var fakeAccessionCommentParentOne = FakeAccessionComment.Generate(new FakeAccessionCommentForCreationDto().Generate());
        await InsertAsync(fakeAccessionCommentParentOne);

        var fakeAccessionComment = FakeAccessionComment.Generate(new FakeAccessionCommentForCreationDto()
            .RuleFor(a => a.AccessionId, _ => fakeAccessionOne.Id)
            .RuleFor(a => a.OriginalCommentId, _ => fakeAccessionCommentParentOne.Id).Generate());

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeAccessionComment);

        // Act
        var route = ApiRoutes.AccessionComments.GetRecord.Replace(ApiRoutes.AccessionComments.Id, fakeAccessionComment.Id.ToString());
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
            
    [Test]
    public async Task get_accessioncomment_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeAccessionComment = FakeAccessionComment.Generate(new FakeAccessionCommentForCreationDto().Generate());

        await InsertAsync(fakeAccessionComment);

        // Act
        var route = ApiRoutes.AccessionComments.GetRecord.Replace(ApiRoutes.AccessionComments.Id, fakeAccessionComment.Id.ToString());
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Test]
    public async Task get_accessioncomment_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeAccessionComment = FakeAccessionComment.Generate(new FakeAccessionCommentForCreationDto().Generate());
        FactoryClient.AddAuth();

        await InsertAsync(fakeAccessionComment);

        // Act
        var route = ApiRoutes.AccessionComments.GetRecord.Replace(ApiRoutes.AccessionComments.Id, fakeAccessionComment.Id.ToString());
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}