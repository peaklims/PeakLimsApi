namespace PeakLims.IntegrationTests.FeatureTests.AccessionComments;

using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using PeakLims.Domain.AccessionComments.Dtos;
using SharedKernel.Exceptions;
using PeakLims.Domain.AccessionComments.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class UpdateAccessionCommentCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_accessioncomment_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeAccessionCommentOne = new FakeAccessionCommentBuilder().Build();
        var updatedAccessionCommentDto = new FakeAccessionCommentForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakeAccessionCommentOne);

        var accessionComment = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionComments
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionCommentOne.Id));

        // Act
        var command = new UpdateAccessionComment.Command(accessionComment.Id, updatedAccessionCommentDto);
        await testingServiceScope.SendAsync(command);
        var updatedAccessionComment = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionComments.FirstOrDefaultAsync(a => a.Id == accessionComment.Id));

        // Assert
        updatedAccessionComment.Comment.Should().Be(updatedAccessionCommentDto.Comment);
        updatedAccessionComment.Status.Should().Be(updatedAccessionCommentDto.Status);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdateAccessionComments);
        var fakeAccessionCommentOne = new FakeAccessionCommentForUpdateDto();

        // Act
        var command = new UpdateAccessionComment.Command(Guid.NewGuid(), fakeAccessionCommentOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}