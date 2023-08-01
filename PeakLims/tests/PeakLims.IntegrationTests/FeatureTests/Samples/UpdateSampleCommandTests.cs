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
        updatedSample.SampleNumber.Should().Be(updatedSampleDto.SampleNumber);
        updatedSample.Status.Should().Be(updatedSampleDto.Status);
        updatedSample.Type.Should().Be(updatedSampleDto.Type);
        updatedSample.Quantity.Should().Be(updatedSampleDto.Quantity);
        updatedSample.CollectionDate.Should().Be(updatedSampleDto.CollectionDate);
        updatedSample.ReceivedDate.Should().Be(updatedSampleDto.ReceivedDate);
        updatedSample.CollectionSite.Should().Be(updatedSampleDto.CollectionSite);
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