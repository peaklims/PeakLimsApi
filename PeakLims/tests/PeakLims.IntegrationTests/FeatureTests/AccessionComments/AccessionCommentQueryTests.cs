namespace PeakLims.IntegrationTests.FeatureTests.AccessionComments;

using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using PeakLims.Domain.AccessionComments.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Exceptions;

public class AccessionCommentQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_accessioncomment_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionCommentOne = new FakeAccessionCommentBuilder().Build();
        await testingServiceScope.InsertAsync(fakeAccessionCommentOne);

        // Act
        var query = new GetAccessionComment.Query(fakeAccessionCommentOne.Id);
        var accessionComment = await testingServiceScope.SendAsync(query);

        // Assert
        accessionComment.Comment.Should().Be(fakeAccessionCommentOne.Comment);
        accessionComment.Status.Should().Be(fakeAccessionCommentOne.Status);
    }

    [Fact]
    public async Task get_accessioncomment_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetAccessionComment.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_exclude_comment_from_another_org()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionCommentOne = new FakeAccessionCommentBuilder().Build();
        await testingServiceScope.InsertAsync(fakeAccessionCommentOne);
        
        testingServiceScope.SetRandomUserInNewOrg();

        // Act
        var query = new GetAccessionComment.Query(fakeAccessionCommentOne.Id);
        var act = async () => await testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadAccessionComments);

        // Act
        var command = new GetAccessionComment.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}