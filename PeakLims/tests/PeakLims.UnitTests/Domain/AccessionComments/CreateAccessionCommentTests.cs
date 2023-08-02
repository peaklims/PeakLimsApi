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
        var accession = new FakeAccessionBuilder().Build();
        var comment = _faker.Lorem.Sentence();
        
        // Act
        var accessionComment = AccessionComment.Create(accession, comment);

        // Assert
        accessionComment.Comment.Should().Be(comment);
        accessionComment.Accession.Id.Should().Be(accession.Id);
        accessionComment.ParentComment.Should().BeNull();
        accessionComment.ParentComment.Should().BeNull();
        accessionComment.Status.Should().Be(AccessionCommentStatus.Active());
    }
    
    [Fact]
    public void must_have_a_comment()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();
        
        // Act
        var act = () => AccessionComment.Create(accession, null);

        // Assert
        act.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage("Please provide a valid comment.");
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();
        var comment = _faker.Lorem.Sentence();
        
        // Act
        var fakeAccessionComment = AccessionComment.Create(accession, comment);

        // Assert
        fakeAccessionComment.DomainEvents.Count.Should().Be(1);
        fakeAccessionComment.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(AccessionCommentCreated));
    }
}