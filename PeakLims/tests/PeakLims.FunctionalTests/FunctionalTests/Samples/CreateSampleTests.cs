namespace PeakLims.FunctionalTests.FunctionalTests.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class CreateSampleTests : TestBase
{
    [Fact]
    public async Task create_sample_returns_created_using_valid_dto_and_valid_auth_credentials()
    {
        // Arrange
        var fakeSample = new FakeSampleForCreationDto().Generate();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);

        // Act
        var route = ApiRoutes.Samples.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeSample);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }
            
    [Fact]
    public async Task create_sample_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeSample = new FakeSampleForCreationDto { }.Generate();

        // Act
        var route = ApiRoutes.Samples.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeSample);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task create_sample_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeSample = new FakeSampleForCreationDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Samples.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeSample);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}