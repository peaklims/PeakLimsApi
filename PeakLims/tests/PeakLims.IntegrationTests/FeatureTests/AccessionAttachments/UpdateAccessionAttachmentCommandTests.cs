namespace PeakLims.IntegrationTests.FeatureTests.AccessionAttachments;

using PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;
using PeakLims.Domain.AccessionAttachments.Dtos;
using PeakLims.Domain.AccessionAttachments.Features;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class UpdateAccessionAttachmentCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_accessionattachment_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var accessionAttachmentOne = new FakeAccessionAttachmentBuilder().Build();
        var updatedAccessionAttachmentDto = new FakeAccessionAttachmentForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(accessionAttachmentOne);

        var accessionAttachment = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionAttachments
            .FirstOrDefaultAsync(a => a.Id == accessionAttachmentOne.Id));

        // Act
        var command = new UpdateAccessionAttachment.Command(accessionAttachment.Id, updatedAccessionAttachmentDto);
        await testingServiceScope.SendAsync(command);
        var updatedAccessionAttachment = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionAttachments.FirstOrDefaultAsync(a => a.Id == accessionAttachment.Id));

        // Assert
        updatedAccessionAttachment.Type.Value.Should().Be(updatedAccessionAttachmentDto.Type);
        updatedAccessionAttachment.Comments.Should().Be(updatedAccessionAttachmentDto.Comments);
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdateAccessionAttachments);
        var accessionAttachmentOne = new FakeAccessionAttachmentForUpdateDto();

        // Act
        var command = new UpdateAccessionAttachment.Command(Guid.NewGuid(), accessionAttachmentOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}