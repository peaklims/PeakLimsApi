namespace PeakLims.IntegrationTests.FeatureTests.AccessionComments;

using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using PeakLims.Domain.AccessionComments.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class DeleteAccessionCommentCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_accessioncomment_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionCommentOne = new FakeAccessionCommentBuilder().Build();
        await testingServiceScope.InsertAsync(fakeAccessionCommentOne);
        var accessionComment = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionComments
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionCommentOne.Id));

        // Act
        var command = new DeleteAccessionComment.Command(accessionComment.Id);
        await testingServiceScope.SendAsync(command);
        var accessionCommentResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionComments.CountAsync(a => a.Id == accessionComment.Id));

        // Assert
        accessionCommentResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_accessioncomment_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeleteAccessionComment.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_accessioncomment_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionCommentOne = new FakeAccessionCommentBuilder().Build();
        await testingServiceScope.InsertAsync(fakeAccessionCommentOne);
        var accessionComment = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionComments
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionCommentOne.Id));

        // Act
        var command = new DeleteAccessionComment.Command(accessionComment.Id);
        await testingServiceScope.SendAsync(command);
        var deletedAccessionComment = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionComments
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == accessionComment.Id));

        // Assert
        deletedAccessionComment?.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeleteAccessionComments);

        // Act
        var command = new DeleteAccessionComment.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}