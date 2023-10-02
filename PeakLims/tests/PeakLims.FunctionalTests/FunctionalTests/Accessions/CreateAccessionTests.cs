namespace PeakLims.FunctionalTests.FunctionalTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class CreateAccessionTests : TestBase
{
    [Fact]
    public async Task create_accession_returns_created_using_valid_dto_and_valid_auth_credentials()
    {
        // Arrange
        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);

        // Act
        var route = ApiRoutes.Accessions.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, null);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }
            
    [Fact]
    public async Task create_accession_returns_unauthorized_without_valid_token()
    {
        // Arrange
        // Act
        var route = ApiRoutes.Accessions.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, null);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task create_accession_returns_forbidden_without_proper_scope()
    {
        // Arrange
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Accessions.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, null);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}