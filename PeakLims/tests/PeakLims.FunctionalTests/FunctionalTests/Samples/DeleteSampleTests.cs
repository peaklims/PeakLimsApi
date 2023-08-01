namespace PeakLims.FunctionalTests.FunctionalTests.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class DeleteSampleTests : TestBase
{
    [Fact]
    public async Task delete_sample_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakeSample = new FakeSampleBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeSample);

        // Act
        var route = ApiRoutes.Samples.Delete(fakeSample.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task delete_sample_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeSample = new FakeSampleBuilder().Build();

        // Act
        var route = ApiRoutes.Samples.Delete(fakeSample.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task delete_sample_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeSample = new FakeSampleBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Samples.Delete(fakeSample.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}