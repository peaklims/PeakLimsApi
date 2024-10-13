namespace PeakLims.IntegrationTests.FeatureTests.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.Domain.Samples.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using Domain;
using System.Threading.Tasks;
using Exceptions;
using SharedTestHelpers.Fakes.Patient;

public class DeleteSampleCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_sample_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var sample = new FakeSampleBuilder().Build();
        var patient = new FakePatientBuilder()
            .Build()
            .AddSample(sample);
        await testingServiceScope.InsertAsync(patient);

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
        var sample = new FakeSampleBuilder().Build();
        var patient = new FakePatientBuilder()
            .Build()
            .AddSample(sample);
        await testingServiceScope.InsertAsync(patient);

        // Act
        var command = new DeleteSample.Command(sample.Id);
        await testingServiceScope.SendAsync(command);
        var isDeleted = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .IgnoreQueryFilters()
            .Where(x => x.Id == sample.Id)
            .Select(x => x.IsDeleted)
            .FirstOrDefaultAsync());

        // Assert
        isDeleted.Should().BeTrue();
    }

    [Fact(Skip = "need to redo permission granularity")]
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