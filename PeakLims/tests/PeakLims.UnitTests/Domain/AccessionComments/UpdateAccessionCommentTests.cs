namespace PeakLims.UnitTests.Domain.AccessionComments;

using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using PeakLims.Domain.AccessionComments;
using PeakLims.Domain.AccessionComments.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class UpdateAccessionCommentTests
{
    private readonly Faker _faker;

    public UpdateAccessionCommentTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_accessionComment()
    {
        // Arrange
        var fakeAccessionComment = new FakeAccessionCommentBuilder().Build();
        var updatedAccessionComment = new FakeAccessionCommentForUpdate().Generate();
        
        // Act
        fakeAccessionComment.Update(updatedAccessionComment);

        // Assert
        fakeAccessionComment.Comment.Should().Be(updatedAccessionComment.Comment);
        fakeAccessionComment.Status.Should().Be(updatedAccessionComment.Status);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakeAccessionComment = new FakeAccessionCommentBuilder().Build();
        var updatedAccessionComment = new FakeAccessionCommentForUpdate().Generate();
        fakeAccessionComment.DomainEvents.Clear();
        
        // Act
        fakeAccessionComment.Update(updatedAccessionComment);

        // Assert
        fakeAccessionComment.DomainEvents.Count.Should().Be(1);
        fakeAccessionComment.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(AccessionCommentUpdated));
    }
}