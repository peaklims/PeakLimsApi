namespace PeakLims.FunctionalTests.FunctionalTests.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class GetSampleTests : TestBase
{
    [Fact]
    public async Task get_sample_returns_success_when_entity_exists_using_valid_auth_credentials()
    {
        // Arrange
        var fakeSample = new FakeSampleBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeSample);

        // Act
        var route = ApiRoutes.Samples.GetRecord(fakeSample.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
            
    [Fact]
    public async Task get_sample_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeSample = new FakeSampleBuilder().Build();

        // Act
        var route = ApiRoutes.Samples.GetRecord(fakeSample.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task get_sample_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeSample = new FakeSampleBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Samples.GetRecord(fakeSample.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}