namespace PeakLims.FunctionalTests.FunctionalTests.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class UpdateSampleRecordTests : TestBase
{
    [Fact]
    public async Task put_sample_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakeSample = new FakeSampleBuilder().Build();
        var updatedSampleDto = new FakeSampleForUpdateDto().Generate();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeSample);

        // Act
        var route = ApiRoutes.Samples.Put(fakeSample.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedSampleDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task put_sample_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeSample = new FakeSampleBuilder().Build();
        var updatedSampleDto = new FakeSampleForUpdateDto { }.Generate();

        // Act
        var route = ApiRoutes.Samples.Put(fakeSample.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedSampleDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task put_sample_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeSample = new FakeSampleBuilder().Build();
        var updatedSampleDto = new FakeSampleForUpdateDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Samples.Put(fakeSample.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedSampleDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}