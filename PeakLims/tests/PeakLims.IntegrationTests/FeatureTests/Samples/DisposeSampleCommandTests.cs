namespace PeakLims.IntegrationTests.FeatureTests.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.Domain.Samples.Dtos;
using SharedKernel.Exceptions;
using PeakLims.Domain.Samples.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Domain.SampleStatuses;
using SharedTestHelpers.Fakes.Container;

public class DisposeSampleCommandTests : TestBase
{
    [Fact]
    public async Task can_dispose_existing_sample_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeSampleOne = new FakeSampleBuilder().Build();
        await testingServiceScope.InsertAsync(fakeSampleOne);

        var sample = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .FirstOrDefaultAsync(s => s.Id == fakeSampleOne.Id));

        // Act
        var command = new DisposeSample.Command(sample.Id);
        await testingServiceScope.SendAsync(command);
        var updatedSample = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples.FirstOrDefaultAsync(s => s.Id == sample.Id));

        // Assert
        updatedSample.Status.Should().Be(SampleStatus.Disposed());
    }
}