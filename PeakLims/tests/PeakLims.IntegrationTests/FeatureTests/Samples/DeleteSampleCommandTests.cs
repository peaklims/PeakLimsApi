namespace PeakLims.IntegrationTests.FeatureTests.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.Domain.Samples.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class DeleteSampleCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_sample_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeSampleOne = new FakeSampleBuilder().Build();
        await testingServiceScope.InsertAsync(fakeSampleOne);
        var sample = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .FirstOrDefaultAsync(s => s.Id == fakeSampleOne.Id));

        // Act
        var command = new DeleteSample.Command(sample.Id);
        await testingServiceScope.SendAsync(command);
        var sampleResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples.CountAsync(s => s.Id == sample.Id));

        // Assert
        sampleResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_sample_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeleteSample.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_sample_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeSampleOne = new FakeSampleBuilder().Build();
        await testingServiceScope.InsertAsync(fakeSampleOne);
        var sample = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .FirstOrDefaultAsync(s => s.Id == fakeSampleOne.Id));

        // Act
        var command = new DeleteSample.Command(sample.Id);
        await testingServiceScope.SendAsync(command);
        var deletedSample = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == sample.Id));

        // Assert
        deletedSample?.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeleteSamples);

        // Act
        var command = new DeleteSample.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}