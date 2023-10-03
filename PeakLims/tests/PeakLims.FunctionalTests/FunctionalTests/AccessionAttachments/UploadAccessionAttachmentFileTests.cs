namespace PeakLims.FunctionalTests.FunctionalTests.AccessionAttachments;

using PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using System.Net;
using System.Threading.Tasks;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.Files;

public class UploadAccessionAttachmentFileTests : TestBase
{
    [Fact]
    public async Task can_upload_attachment()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();
        await InsertAsync(accession);
        var file = new FakeFileBuilder().Build();

        var callingUser = await AddNewSuperAdmin();
        FactoryClient.AddAuth(callingUser.Identifier);

        var content = new MultipartFormDataContent
        {
            { new StreamContent(file.OpenReadStream()), "file", file.FileName }
        };

        var route = ApiRoutes.AccessionAttachments.Upload(accession.Id);

        // Act
        var result = await FactoryClient.PostAsync(route, content);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task put_accessionattachment_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();
        await InsertAsync(accession);
        var file = new FakeFileBuilder().Build();
        
        var content = new MultipartFormDataContent
        {
            { new StreamContent(file.OpenReadStream()), "file", file.FileName }
        };
        
        var route = ApiRoutes.AccessionAttachments.Upload(accession.Id);
        
        // Act
        var result = await FactoryClient.PostAsync(route, content);
    
        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task put_accessionattachment_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();
        await InsertAsync(accession);
        var file = new FakeFileBuilder().Build();
        
        var content = new MultipartFormDataContent
        {
            { new StreamContent(file.OpenReadStream()), "file", file.FileName }
        };
        
        FactoryClient.AddAuth();
        var route = ApiRoutes.AccessionAttachments.Upload(accession.Id);
    
        // Act
        var result = await FactoryClient.PostAsync(route, content);
    
        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}