namespace PeakLims.UnitTests.Domain.AccessionComments;

using PeakLims.SharedTestHelpers.Fakes.AccessionComment;
using PeakLims.Domain.AccessionComments;
using PeakLims.Domain.AccessionComments.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class CreateAccessionCommentTests
{
    private readonly Faker _faker;

    public CreateAccessionCommentTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_accessionComment()
    {
        // Arrange
        var accessionCommentToCreate = new FakeAccessionCommentForCreation().Generate();
        
        // Act
        var fakeAccessionComment = AccessionComment.Create(accessionCommentToCreate);

        // Assert
        fakeAccessionComment.Comment.Should().Be(accessionCommentToCreate.Comment);
        fakeAccessionComment.Status.Should().Be(accessionCommentToCreate.Status);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var accessionCommentToCreate = new FakeAccessionCommentForCreation().Generate();
        
        // Act
        var fakeAccessionComment = AccessionComment.Create(accessionCommentToCreate);

        // Assert
        fakeAccessionComment.DomainEvents.Count.Should().Be(1);
        fakeAccessionComment.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(AccessionCommentCreated));
    }
}