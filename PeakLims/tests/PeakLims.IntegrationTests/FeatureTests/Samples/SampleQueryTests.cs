namespace PeakLims.IntegrationTests.FeatureTests.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.Domain.Samples.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Exceptions;
using SharedTestHelpers.Fakes.Patient;

public class SampleQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_sample_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var sample = new FakeSampleBuilder().Build();
        var patient = new FakePatientBuilder()
            .Build()
            .AddSample(sample);
        await testingServiceScope.InsertAsync(patient);

        // Act
        var query = new GetSample.Query(sample.Id);
        var sampleResponse = await testingServiceScope.SendAsync(query);

        // Assert
        sampleResponse.SampleNumber.Should().Be(sample.SampleNumber);
        sampleResponse.ExternalId.Should().Be(sample.ExternalId);
        sampleResponse.Status.Should().Be(sample.Status);
        sampleResponse.Type.Should().Be(sample.Type);
        sampleResponse.Quantity.Should().Be(sample.Quantity);
        sampleResponse.CollectionDate.Should().Be(sample.CollectionDate);
        sampleResponse.ReceivedDate.Should().Be(sample.ReceivedDate);
        sampleResponse.CollectionSite.Should().Be(sample.CollectionSite);
        sampleResponse.SampleNumber.Should().NotBeNull();
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

    [Fact(Skip = "need to redo permission granularity")]
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