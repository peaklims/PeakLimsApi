namespace PeakLims.UnitTests.Domain.AccessionAttachments;

using PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;
using PeakLims.Domain.AccessionAttachments;
using PeakLims.Domain.AccessionAttachments.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using ValidationException = PeakLims.Exceptions.ValidationException;

public class CreateAccessionAttachmentTests
{
    private readonly Faker _faker;

    public CreateAccessionAttachmentTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_accessionAttachment()
    {
        // Arrange
        var accessionAttachmentToCreate = new FakeAccessionAttachmentForCreation().Generate();
        
        // Act
        var accessionAttachment = AccessionAttachment.Create(accessionAttachmentToCreate);

        // Assert
        accessionAttachment.Type.Value.Should().Be(accessionAttachmentToCreate.Type);
        accessionAttachment.Comments.Should().Be(accessionAttachmentToCreate.Comments);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var accessionAttachmentToCreate = new FakeAccessionAttachmentForCreation().Generate();
        
        // Act
        var accessionAttachment = AccessionAttachment.Create(accessionAttachmentToCreate);

        // Assert
        accessionAttachment.DomainEvents.Count.Should().Be(1);
        accessionAttachment.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(AccessionAttachmentCreated));
    }
}