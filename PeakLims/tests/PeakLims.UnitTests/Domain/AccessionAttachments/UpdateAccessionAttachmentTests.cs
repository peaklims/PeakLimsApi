namespace PeakLims.UnitTests.Domain.AccessionAttachments;

using PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;
using PeakLims.Domain.AccessionAttachments;
using PeakLims.Domain.AccessionAttachments.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using ValidationException = PeakLims.Exceptions.ValidationException;

public class UpdateAccessionAttachmentTests
{
    private readonly Faker _faker;

    public UpdateAccessionAttachmentTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_accessionAttachment()
    {
        // Arrange
        var accessionAttachment = new FakeAccessionAttachmentBuilder().Build();
        var updatedAccessionAttachment = new FakeAccessionAttachmentForUpdate().Generate();
        
        // Act
        accessionAttachment.Update(updatedAccessionAttachment);

        // Assert
        accessionAttachment.Type.Value.Should().Be(updatedAccessionAttachment.Type);
        accessionAttachment.Comments.Should().Be(updatedAccessionAttachment.Comments);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var accessionAttachment = new FakeAccessionAttachmentBuilder().Build();
        var updatedAccessionAttachment = new FakeAccessionAttachmentForUpdate().Generate();
        accessionAttachment.DomainEvents.Clear();
        
        // Act
        accessionAttachment.Update(updatedAccessionAttachment);

        // Assert
        accessionAttachment.DomainEvents.Count.Should().Be(1);
        accessionAttachment.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(AccessionAttachmentUpdated));
    }
}