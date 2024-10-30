namespace PeakLims.IntegrationTests.FeatureTests.AccessionComments;

using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using PeakLims.Domain.AccessionComments.Dtos;
using PeakLims.Domain.AccessionComments.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Bogus;
using Domain.AccessionCommentStatuses;
using Exceptions;
using Services;
using SharedTestHelpers.Utilities;

public class UpdateAccessionCommentCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_accessioncomment_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var currentUserService = testingServiceScope.GetService<ICurrentUserService>();
        var originalAccessionComment = new FakeAccessionCommentBuilder()
            .WithUserIdentifier(currentUserService.UserIdentifier)
            .Build();
        await testingServiceScope.InsertAsync(originalAccessionComment);
        
        var faker = new Faker();
        var comment = faker.Lorem.Sentence();

        // Act
        var command = new UpdateAccessionComment.Command(originalAccessionComment.Id, comment, originalAccessionComment.CreatedBy);
        await testingServiceScope.SendAsync(command);
        
        var accessionComments = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionComments
            .Where(a => a.Accession.Id == originalAccessionComment.Accession.Id)
            .ToListAsync());
        var newComment = accessionComments.FirstOrDefault(a => a.Status == AccessionCommentStatus.Active());
        var archivedComment = accessionComments.FirstOrDefault(a => a.Status == AccessionCommentStatus.Archived());
        
        // Assert
        accessionComments.Count.Should().Be(2);
        
        newComment.Accession.Id.Should().Be(originalAccessionComment.Accession.Id);
        newComment.Comment.Should().Be(comment);
        newComment.ParentComment.Should().BeNull();
        newComment.Status.Should().Be(AccessionCommentStatus.Active());
        
        archivedComment.Id.Should().Be(originalAccessionComment.Id);
        archivedComment.Accession.Id.Should().Be(originalAccessionComment.Accession.Id);
        archivedComment.ParentComment.Id.Should().Be(newComment.Id);
        archivedComment.Comment.Should().Be(originalAccessionComment.Comment);
        archivedComment.Status.Should().Be(AccessionCommentStatus.Archived());
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdateAccessionComments);
        var faker = new Faker();
        var comment = faker.Lorem.Sentence();

        // Act
        var command = new UpdateAccessionComment.Command(Guid.NewGuid(), comment, null);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task can_not_edit_another_user_comment()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var faker = new Faker();
        var originalAccessionComment = new FakeAccessionCommentBuilder().Build();
        await testingServiceScope.InsertAsync(originalAccessionComment);
        
        var comment = faker.Lorem.Sentence();

        // Act
        var command = new UpdateAccessionComment.Command(originalAccessionComment.Id, comment, Guid.NewGuid().ToString());
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}