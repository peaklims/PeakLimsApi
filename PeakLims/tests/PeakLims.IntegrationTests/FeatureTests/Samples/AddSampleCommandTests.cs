namespace PeakLims.IntegrationTests.FeatureTests.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using PeakLims.Domain.Samples.Features;
using SharedKernel.Exceptions;

public class AddSampleCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_sample_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeSampleOne = new FakeSampleForCreationDto().Generate();

        // Act
        var command = new AddSample.Command(fakeSampleOne);
        var sampleReturned = await testingServiceScope.SendAsync(command);
        var sampleCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .FirstOrDefaultAsync(s => s.Id == sampleReturned.Id));

        // Assert
        sampleReturned.SampleNumber.Should().Be(fakeSampleOne.SampleNumber);
        sampleReturned.Status.Should().Be(fakeSampleOne.Status);
        sampleReturned.Type.Should().Be(fakeSampleOne.Type);
        sampleReturned.Quantity.Should().Be(fakeSampleOne.Quantity);
        sampleReturned.CollectionDate.Should().Be(fakeSampleOne.CollectionDate);
        sampleReturned.ReceivedDate.Should().Be(fakeSampleOne.ReceivedDate);
        sampleReturned.CollectionSite.Should().Be(fakeSampleOne.CollectionSite);

        sampleCreated.SampleNumber.Should().Be(fakeSampleOne.SampleNumber);
        sampleCreated.Status.Should().Be(fakeSampleOne.Status);
        sampleCreated.Type.Should().Be(fakeSampleOne.Type);
        sampleCreated.Quantity.Should().Be(fakeSampleOne.Quantity);
        sampleCreated.CollectionDate.Should().Be(fakeSampleOne.CollectionDate);
        sampleCreated.ReceivedDate.Should().Be(fakeSampleOne.ReceivedDate);
        sampleCreated.CollectionSite.Should().Be(fakeSampleOne.CollectionSite);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddSamples);
        var fakeSampleOne = new FakeSampleForCreationDto();

        // Act
        var command = new AddSample.Command(fakeSampleOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}