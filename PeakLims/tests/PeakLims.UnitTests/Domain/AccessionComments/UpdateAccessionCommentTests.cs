namespace PeakLims.UnitTests.Domain.AccessionComments;

using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using PeakLims.Domain.AccessionComments;
using PeakLims.Domain.AccessionComments.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using PeakLims.Domain.AccessionCommentStatuses;
using SharedTestHelpers.Fakes.Accession;
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
        var originalAccessionComment = new FakeAccessionCommentBuilder().Build();
        var comment = _faker.Lorem.Sentence();
        
        // Act
        originalAccessionComment.Update(comment, out var newComment, out var archivedComment);

        // Assert
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
    public void must_have_a_comment()
    {
        // Arrange
        var originalAccessionComment = new FakeAccessionCommentBuilder().Build();
        
        // Act
        var act = () => originalAccessionComment.Update( null, out _ , out _);

        // Assert
        act.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage("Please provide a valid comment.");
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakeAccessionComment = new FakeAccessionCommentBuilder().Build();
        var comment = _faker.Lorem.Sentence();
        fakeAccessionComment.DomainEvents.Clear();
        
        // Act
        fakeAccessionComment.Update(comment, out var _, out var _);

        // Assert
        fakeAccessionComment.DomainEvents.Count.Should().Be(1);
        fakeAccessionComment.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(AccessionCommentUpdated));
    }
}