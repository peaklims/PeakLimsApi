namespace PeakLims.IntegrationTests.FeatureTests.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.Domain.Samples.Dtos;
using SharedKernel.Exceptions;
using PeakLims.Domain.Samples.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using SharedTestHelpers.Fakes.Container;

public class UpdateSampleCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_sample_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeSampleOne = new FakeSampleBuilder().Build();
        var updatedSampleDto = new FakeSampleForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakeSampleOne);

        var sample = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .FirstOrDefaultAsync(s => s.Id == fakeSampleOne.Id));

        // Act
        var command = new UpdateSample.Command(sample.Id, updatedSampleDto);
        await testingServiceScope.SendAsync(command);
        var updatedSample = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples.FirstOrDefaultAsync(s => s.Id == sample.Id));

        // Assert
        updatedSample.Type.Value.Should().Be(updatedSampleDto.Type);
        updatedSample.Status.Should().Be(updatedSampleDto.Status);
        updatedSample.Quantity.Should().Be(updatedSampleDto.Quantity);
        updatedSample.CollectionDate.Should().Be(updatedSampleDto.CollectionDate);
        updatedSample.ReceivedDate.Should().Be(updatedSampleDto.ReceivedDate);
        updatedSample.CollectionSite.Should().Be(updatedSampleDto.CollectionSite);
        updatedSample.SampleNumber.Should().NotBeNull();
    }
    
    [Fact]
    public async Task can_update_existing_sample_in_db_with_container()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        
        var container = new FakeContainerBuilder().Build();
        await testingServiceScope.InsertAsync(container);
        
        var fakeSampleOne = new FakeSampleBuilder().Build();
        await testingServiceScope.InsertAsync(fakeSampleOne);
        
        var updatedSampleDto = new FakeSampleForUpdateDto()
            .RuleFor(x => x.Type, f => container.UsedFor.Value)
            .Generate();
        updatedSampleDto.ContainerId = container.Id;

        var sample = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .FirstOrDefaultAsync(s => s.Id == fakeSampleOne.Id));

        // Act
        var command = new UpdateSample.Command(sample.Id, updatedSampleDto);
        await testingServiceScope.SendAsync(command);
        var updatedSample = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples.FirstOrDefaultAsync(s => s.Id == sample.Id));

        // Assert
        updatedSample.Container.Id.Should().Be(container.Id);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdateSamples);
        var fakeSampleOne = new FakeSampleForUpdateDto();

        // Act
        var command = new UpdateSample.Command(Guid.NewGuid(), fakeSampleOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}