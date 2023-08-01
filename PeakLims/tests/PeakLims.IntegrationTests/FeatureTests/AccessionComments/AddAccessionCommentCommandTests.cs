namespace PeakLims.IntegrationTests.FeatureTests.AccessionComments;

using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using PeakLims.Domain.AccessionComments.Features;
using SharedKernel.Exceptions;

public class AddAccessionCommentCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_accessioncomment_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionCommentOne = new FakeAccessionCommentForCreationDto().Generate();

        // Act
        var command = new AddAccessionComment.Command(fakeAccessionCommentOne);
        var accessionCommentReturned = await testingServiceScope.SendAsync(command);
        var accessionCommentCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionComments
            .FirstOrDefaultAsync(a => a.Id == accessionCommentReturned.Id));

        // Assert
        accessionCommentReturned.Comment.Should().Be(fakeAccessionCommentOne.Comment);
        accessionCommentReturned.Status.Should().Be(fakeAccessionCommentOne.Status);

        accessionCommentCreated.Comment.Should().Be(fakeAccessionCommentOne.Comment);
        accessionCommentCreated.Status.Should().Be(fakeAccessionCommentOne.Status);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddAccessionComments);
        var fakeAccessionCommentOne = new FakeAccessionCommentForCreationDto();

        // Act
        var command = new AddAccessionComment.Command(fakeAccessionCommentOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}