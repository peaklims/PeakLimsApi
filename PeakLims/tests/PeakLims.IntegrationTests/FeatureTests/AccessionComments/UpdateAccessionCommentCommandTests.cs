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
using Bogus;
using Domain.AccessionCommentStatuses;

public class UpdateAccessionCommentCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_accessioncomment_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var originalAccessionComment = new FakeAccessionCommentBuilder().Build();
        await testingServiceScope.InsertAsync(originalAccessionComment);
        
        var faker = new Faker();
        var comment = faker.Lorem.Sentence();

        // Act
        var command = new UpdateAccessionComment.Command(originalAccessionComment.Id, comment);
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

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdateAccessionComments);
        var faker = new Faker();
        var comment = faker.Lorem.Sentence();

        // Act
        var command = new UpdateAccessionComment.Command(Guid.NewGuid(), comment);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}