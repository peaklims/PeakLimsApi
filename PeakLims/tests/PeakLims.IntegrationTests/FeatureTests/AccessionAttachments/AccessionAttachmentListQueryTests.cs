namespace PeakLims.IntegrationTests.FeatureTests.AccessionAttachments;

using PeakLims.Domain.AccessionAttachments.Dtos;
using PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;
using PeakLims.Domain.AccessionAttachments.Features;
using Domain;
using System.Threading.Tasks;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.Files;

public class AccessionAttachmentListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_accessionattachment_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var queryParameters = new AccessionAttachmentParametersDto();

        var accession = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(accession);

        var file = new FakeFileBuilder().Build();
        var command = new UploadAccessionAttachmentFile.Command(accession.Id, file);
        await testingServiceScope.SendAsync(command);

        // Act
        var query = new GetAccessionAttachmentList.Query(queryParameters);
        var accessionAttachments = await testingServiceScope.SendAsync(query);

        // Assert
        accessionAttachments.Count.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadAccessionAttachments);
        var queryParameters = new AccessionAttachmentParametersDto();

        // Act
        var command = new GetAccessionAttachmentList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}