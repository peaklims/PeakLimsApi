namespace PeakLims.IntegrationTests.FeatureTests.AccessionComments;

using Bogus;
using Domain.AccessionComments.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.AccessionComment;
using Xunit;

public class GetAccessionCommentViewTests : TestBase
{
    [Fact]
    public async Task can_get_view_with_basic_history()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        
        // summary
        var accession = new FakeAccessionBuilder().Build();

        // input
        var accessionCommentItem = new FakeAccessionCommentBuilder()
            .WithAccession(accession)
            .Build();
        await testingServiceScope.InsertAsync(accessionCommentItem);
        var newComment =  new Faker().Lorem.Sentence();
        var command = new UpdateAccessionComment.Command(accessionCommentItem.Id, newComment);
        await testingServiceScope.SendAsync(command);

        // Act
        var query = new GetAccessionCommentView.Query(accession.Id);
        var accessionView = await testingServiceScope.SendAsync(query);

        // Assert
        accessionView.AccessionComments.Count.Should().Be(1);
        var viewCommentItem = accessionView.AccessionComments[0];
        viewCommentItem.Comment.Should().Be(newComment);
        viewCommentItem.History.Count.Should().Be(1);
        
        var viewCommentItemHistory = viewCommentItem.History[0];
        viewCommentItemHistory.Comment.Should().Be(accessionCommentItem.Comment);
    }
    
    [Fact]
    public async Task can_handle_complex_chat_history()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        
        // summary
        var accession = new FakeAccessionBuilder().Build();

        // standalone input
        var standaloneCommentItem = new FakeAccessionCommentBuilder()
            .WithAccession(accession)
            .Build();
        
        // input with history
        var rootCommentItem = new FakeAccessionCommentBuilder()
            .WithAccession(accession)
            .Build();
        await testingServiceScope.InsertAsync(rootCommentItem, standaloneCommentItem);
        
        var firstUpdatedCommentText = new Faker().Lorem.Sentence();
        var command = new UpdateAccessionComment.Command(rootCommentItem.Id, firstUpdatedCommentText);
        await testingServiceScope.SendAsync(command);
        
        var updatedCommentItem = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionComments
            .FirstOrDefaultAsync(x => x.Comment == firstUpdatedCommentText));
        var finalCommentText = new Faker().Lorem.Sentence();
        command = new UpdateAccessionComment.Command(updatedCommentItem.Id, finalCommentText);
        await testingServiceScope.SendAsync(command);

        // Act
        var query = new GetAccessionCommentView.Query(accession.Id);
        var accessionView = await testingServiceScope.SendAsync(query);

        // Assert
        accessionView.AccessionComments.Count.Should().Be(2);
        var standaloneCommentItemView = accessionView.AccessionComments.FirstOrDefault(x => x.Id == standaloneCommentItem.Id);
        standaloneCommentItemView.Comment.Should().Be(standaloneCommentItem.Comment);
        
        var editedCommentItemView = accessionView.AccessionComments.FirstOrDefault(x => x.Id != standaloneCommentItem.Id);
        editedCommentItemView.Comment.Should().Be(finalCommentText);
 
        editedCommentItemView.History.Count.Should().Be(2);
        editedCommentItemView.History.Select(x => x.Comment).Should().Contain(rootCommentItem.Comment);
        editedCommentItemView.History.Select(x => x.Comment).Should().Contain(firstUpdatedCommentText);
    }
}