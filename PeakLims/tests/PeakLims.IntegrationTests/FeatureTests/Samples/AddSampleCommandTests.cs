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
using SharedTestHelpers.Fakes.Container;

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
        sampleReturned.Type.Should().Be(fakeSampleOne.Type);
        sampleReturned.Quantity.Should().Be(fakeSampleOne.Quantity);
        sampleReturned.Status.Should().Be(fakeSampleOne.Status);
        sampleReturned.CollectionDate.Should().Be(fakeSampleOne.CollectionDate);
        sampleReturned.ReceivedDate.Should().Be(fakeSampleOne.ReceivedDate);
        sampleReturned.CollectionSite.Should().Be(fakeSampleOne.CollectionSite);

        sampleCreated.Type.Value.Should().Be(fakeSampleOne.Type);
        sampleCreated.Quantity.Should().Be(fakeSampleOne.Quantity);
        sampleCreated.Status.Should().Be(fakeSampleOne.Status);
        sampleCreated.CollectionDate.Should().Be(fakeSampleOne.CollectionDate);
        sampleCreated.ReceivedDate.Should().Be(fakeSampleOne.ReceivedDate);
        sampleCreated.CollectionSite.Should().Be(fakeSampleOne.CollectionSite);
    }
    
    [Fact]
    public async Task can_add_new_sample_to_db_with_container()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var container = new FakeContainerBuilder().Build();
        await testingServiceScope.InsertAsync(container);
        var fakeSampleOne = new FakeSampleForCreationDto().Generate();
        fakeSampleOne.ContainerId = container.Id;
        fakeSampleOne.Type = container.UsedFor.Value;

        // Act
        var command = new AddSample.Command(fakeSampleOne);
        var sampleReturned = await testingServiceScope.SendAsync(command);
        var sampleCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .FirstOrDefaultAsync(s => s.Id == sampleReturned.Id));

        // Assert
        sampleReturned.Type.Should().Be(fakeSampleOne.Type);
        sampleReturned.Quantity.Should().Be(fakeSampleOne.Quantity);
        sampleReturned.Status.Should().Be(fakeSampleOne.Status);
        sampleReturned.CollectionDate.Should().Be(fakeSampleOne.CollectionDate);
        sampleReturned.ReceivedDate.Should().Be(fakeSampleOne.ReceivedDate);
        sampleReturned.CollectionSite.Should().Be(fakeSampleOne.CollectionSite);
        sampleReturned.ContainerId.Should().Be(container.Id);
        sampleReturned.SampleNumber.Should().NotBeNull();

        sampleCreated.Type.Value.Should().Be(fakeSampleOne.Type);
        sampleCreated.Quantity.Should().Be(fakeSampleOne.Quantity);
        sampleCreated.Status.Should().Be(fakeSampleOne.Status);
        sampleCreated.CollectionDate.Should().Be(fakeSampleOne.CollectionDate);
        sampleCreated.ReceivedDate.Should().Be(fakeSampleOne.ReceivedDate);
        sampleCreated.CollectionSite.Should().Be(fakeSampleOne.CollectionSite);
        sampleCreated.Container.Id.Should().Be(container.Id);
        sampleCreated.SampleNumber.Should().NotBeNull();
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