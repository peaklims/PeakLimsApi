namespace PeakLims.IntegrationTests.FeatureTests.AccessionAttachments;

using PeakLims.Domain.AccessionAttachments.Dtos;
using PeakLims.Domain.AccessionAttachments.Features;
using Domain;
using System.Threading.Tasks;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.ClaimsPrincipal;
using SharedTestHelpers.Fakes.Files;
using SharedTestHelpers.Fakes.PeakOrganization;

public class AccessionAttachmentListQueryTests : TestBase
{

    [Fact]
    public async Task can_get_accessionattachment_list()
    {
        // Arrange
        // Attachments are org-scoped (via Accession.OrganizationId) and the list projection computes a
        // pre-signed URL for every attachment it returns. Running in a dedicated organization isolates
        // this test from attachments other tests leave in the shared org (some without an S3 bucket set),
        // which keeps it deterministic and parallel-safe.
        var testingServiceScope = new TestingServiceScope();
        var org = new FakePeakOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(org);
        testingServiceScope.SetUser(new FakeClaimsPrincipalBuilder().WithOrganizationId(org.Id).Build());

        var accession = new FakeAccessionBuilder().WithOrganizationId(org.Id).Build();
        await testingServiceScope.InsertAsync(accession);

        var file = new FakeFileBuilder().Build();
        var command = new UploadAccessionAttachmentFile.Command(accession.Id, file);
        await testingServiceScope.SendAsync(command);

        var queryParameters = new AccessionAttachmentParametersDto();

        // Act
        var query = new GetAccessionAttachmentList.Query(queryParameters);
        var accessionAttachments = await testingServiceScope.SendAsync(query);

        // Assert
        accessionAttachments.Count.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact(Skip = "need to redo permission granularity")]
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