namespace PeakLims.FunctionalTests.FunctionalTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class DeleteAccessionTests : TestBase
{
    [Fact]
    public async Task delete_accession_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakeAccession = new FakeAccessionBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeAccession);

        // Act
        var route = ApiRoutes.Accessions.Delete(fakeAccession.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task delete_accession_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeAccession = new FakeAccessionBuilder().Build();

        // Act
        var route = ApiRoutes.Accessions.Delete(fakeAccession.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task delete_accession_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeAccession = new FakeAccessionBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Accessions.Delete(fakeAccession.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}