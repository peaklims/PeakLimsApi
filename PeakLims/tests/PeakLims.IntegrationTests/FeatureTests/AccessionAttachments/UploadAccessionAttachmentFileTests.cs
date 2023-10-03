namespace PeakLims.IntegrationTests.FeatureTests.AccessionAttachments;

using PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;
using PeakLims.Domain.AccessionAttachments.Dtos;
using PeakLims.Domain.AccessionAttachments.Features;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.Files;

public class UploadAccessionAttachmentFileTests : TestBase
{
    [Fact]
    public async Task can_update_existing_accessionattachment_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var accession = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(accession);

        var file = new FakeFileBuilder().Build();

        // Act
        var command = new UploadAccessionAttachmentFile.Command(accession.Id, file);
        await testingServiceScope.SendAsync(command);
        var attachment = await testingServiceScope.ExecuteDbContextAsync(db => db.AccessionAttachments
            .Include(x => x.Accession)
            .FirstOrDefaultAsync(a => a.Accession.Id == accession.Id));

        // Assert
        attachment.Should().NotBeNull();
        attachment.Accession.Id.Should().Be(accession.Id);
        attachment.S3Bucket.Should().NotBeNullOrEmpty();
        attachment.S3Key.Value.Should().NotBeNull();
        (await testingServiceScope.FileExistsInS3Async(attachment.S3Bucket, attachment.S3Key.Value))
            .Should().BeTrue();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUploadAccessionAttachments);
        var accession = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(accession);

        var file = new FakeFileBuilder().Build();

        // Act
        var command = new UploadAccessionAttachmentFile.Command(accession.Id, file);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}