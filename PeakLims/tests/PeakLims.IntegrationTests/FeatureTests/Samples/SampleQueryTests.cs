namespace PeakLims.IntegrationTests.FeatureTests.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.Domain.Samples.Features;
using SharedKernel.Exceptions;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class SampleQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_sample_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeSampleOne = new FakeSampleBuilder().Build();
        await testingServiceScope.InsertAsync(fakeSampleOne);

        // Act
        var query = new GetSample.Query(fakeSampleOne.Id);
        var sample = await testingServiceScope.SendAsync(query);

        // Assert
        sample.SampleNumber.Should().Be(fakeSampleOne.SampleNumber);
        sample.Status.Should().Be(fakeSampleOne.Status);
        sample.Type.Should().Be(fakeSampleOne.Type);
        sample.Quantity.Should().Be(fakeSampleOne.Quantity);
        sample.CollectionDate.Should().Be(fakeSampleOne.CollectionDate);
        sample.ReceivedDate.Should().Be(fakeSampleOne.ReceivedDate);
        sample.CollectionSite.Should().Be(fakeSampleOne.CollectionSite);
        sample.SampleNumber.Should().NotBeNull();
    }

    [Fact]
    public async Task get_sample_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetSample.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadSamples);

        // Act
        var command = new GetSample.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}