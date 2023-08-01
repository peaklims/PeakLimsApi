namespace PeakLims.IntegrationTests.FeatureTests.Samples;

using PeakLims.Domain.Samples.Dtos;
using PeakLims.SharedTestHelpers.Fakes.Sample;
using SharedKernel.Exceptions;
using PeakLims.Domain.Samples.Features;
using FluentAssertions;
using Domain;
using Xunit;
using System.Threading.Tasks;

public class SampleListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_sample_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeSampleOne = new FakeSampleBuilder().Build();
        var fakeSampleTwo = new FakeSampleBuilder().Build();
        var queryParameters = new SampleParametersDto();

        await testingServiceScope.InsertAsync(fakeSampleOne, fakeSampleTwo);

        // Act
        var query = new GetSampleList.Query(queryParameters);
        var samples = await testingServiceScope.SendAsync(query);

        // Assert
        samples.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadSamples);
        var queryParameters = new SampleParametersDto();

        // Act
        var command = new GetSampleList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}