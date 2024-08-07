namespace PeakLims.IntegrationTests.FeatureTests.AccessionAttachments;

using PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;
using PeakLims.Domain.AccessionAttachments.Features;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Services.External;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.Files;

public class AccessionAttachmentQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_accessionattachment_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();

        var accession = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(accession);

        var file = new FakeFileBuilder().Build();
        var command = new UploadAccessionAttachmentFile.Command(accession.Id, file);
        await testingServiceScope.SendAsync(command);
        var fileStorage = testingServiceScope.GetService<IFileStorage>();
        
        var attachment = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionAttachments
            .FirstOrDefaultAsync(a => a.Accession.Id == accession.Id));
        
        var updatedData = new FakeAccessionAttachmentForUpdateDto().Generate();
        var updateCommand = new UpdateAccessionAttachment.Command(attachment.Id, updatedData);
        await testingServiceScope.SendAsync(updateCommand);
        
        attachment = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionAttachments
            .FirstOrDefaultAsync(a => a.Accession.Id == accession.Id));
        
        // Act
        var query = new GetAccessionAttachment.Query(attachment.Id);
        var accessionAttachment = await testingServiceScope.SendAsync(query);

        // Assert
        accessionAttachment.Type.Should().Be(attachment.Type);
        accessionAttachment.S3Bucket.Should().Be(attachment.S3Bucket);
        accessionAttachment.S3Key.Should().Be(attachment.S3Key);
        accessionAttachment.Filename.Should().Be(attachment.Filename);
        accessionAttachment.Comments.Should().Be(attachment.Comments);
        accessionAttachment.PreSignedUrl.Should().NotBeNull();
        accessionAttachment.PreSignedUrl.Should().Be(attachment.GetPreSignedUrl(fileStorage));
    }

    [Fact]
    public async Task get_accessionattachment_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetAccessionAttachment.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadAccessionAttachments);

        // Act
        var command = new GetAccessionAttachment.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}