namespace PeakLims.IntegrationTests.FeatureTests.AccessionAttachments;

using PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;
using PeakLims.Domain.AccessionAttachments.Features;
using Microsoft.EntityFrameworkCore;
using Domain;
using System.Threading.Tasks;

public class DeleteAccessionAttachmentCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_accessionattachment_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var accessionAttachmentOne = new FakeAccessionAttachmentBuilder().Build();
        await testingServiceScope.InsertAsync(accessionAttachmentOne);
        var accessionAttachment = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionAttachments
            .FirstOrDefaultAsync(a => a.Id == accessionAttachmentOne.Id));

        // Act
        var command = new DeleteAccessionAttachment.Command(accessionAttachment.Id);
        await testingServiceScope.SendAsync(command);
        var accessionAttachmentResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionAttachments.CountAsync(a => a.Id == accessionAttachment.Id));

        // Assert
        accessionAttachmentResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_accessionattachment_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeleteAccessionAttachment.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_accessionattachment_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var accessionAttachmentOne = new FakeAccessionAttachmentBuilder().Build();
        await testingServiceScope.InsertAsync(accessionAttachmentOne);
        var accessionAttachment = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionAttachments
            .FirstOrDefaultAsync(a => a.Id == accessionAttachmentOne.Id));

        // Act
        var command = new DeleteAccessionAttachment.Command(accessionAttachment.Id);
        await testingServiceScope.SendAsync(command);
        var deletedAccessionAttachment = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionAttachments
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == accessionAttachment.Id));

        // Assert
        deletedAccessionAttachment?.IsDeleted.Should().BeTrue();
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeleteAccessionAttachments);

        // Act
        var command = new DeleteAccessionAttachment.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}