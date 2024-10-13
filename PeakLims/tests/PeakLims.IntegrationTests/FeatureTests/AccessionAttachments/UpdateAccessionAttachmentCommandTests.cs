namespace PeakLims.IntegrationTests.FeatureTests.AccessionAttachments;

using PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;
using PeakLims.Domain.AccessionAttachments.Dtos;
using PeakLims.Domain.AccessionAttachments.Features;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SharedTestHelpers.Fakes.Accession;

public class UpdateAccessionAttachmentCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_accessionattachment_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var updatedAccessionAttachmentDto = new FakeAccessionAttachmentForUpdateDto().Generate();

        var accessionAttachment = new FakeAccessionAttachmentBuilder().Build();
        var accession = new FakeAccessionBuilder().Build();
        accession.AddAccessionAttachment(accessionAttachment);
        await testingServiceScope.InsertAsync(accession);

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