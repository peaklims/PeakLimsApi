namespace PeakLims.IntegrationTests.FeatureTests.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using Domain.SampleStatuses;
using PeakLims.Domain.Samples.Features;
using SharedKernel.Exceptions;
using SharedTestHelpers.Fakes.Container;
using SharedTestHelpers.Fakes.Patient;

public class AddSampleCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_sample_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var patient = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(patient);
        
        var fakeSampleOne = new FakeSampleForCreationDto()
            .RuleFor(x => x.PatientId, f => patient.Id)
            .Generate();

        // Act
        var command = new AddSample.Command(fakeSampleOne);
        var sampleReturned = await testingServiceScope.SendAsync(command);
        var sampleCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.Samples
            .Include(x => x.Patient)
            .FirstOrDefaultAsync(s => s.Id == sampleReturned.Id));

        // Assert
        sampleReturned.Type.Should().Be(fakeSampleOne.Type);
        sampleReturned.Quantity.Should().Be(fakeSampleOne.Quantity);
        sampleReturned.Status.Should().Be(SampleStatus.Received().Value);
        sampleReturned.CollectionDate.Should().Be(fakeSampleOne.CollectionDate);
        sampleReturned.ReceivedDate.Should().Be(fakeSampleOne.ReceivedDate);
        sampleReturned.CollectionSite.Should().Be(fakeSampleOne.CollectionSite);

        sampleCreated.Type.Value.Should().Be(fakeSampleOne.Type);
        sampleCreated.Quantity.Should().Be(fakeSampleOne.Quantity);
        sampleCreated.Status.Should().Be(SampleStatus.Received());
        sampleCreated.CollectionDate.Should().Be(fakeSampleOne.CollectionDate);
        sampleCreated.ReceivedDate.Should().Be(fakeSampleOne.ReceivedDate);
        sampleCreated.CollectionSite.Should().Be(fakeSampleOne.CollectionSite);
        sampleCreated.Patient.Id.Should().Be(patient.Id);
    }
    
    [Fact]
    public async Task can_add_new_sample_to_db_with_container()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var container = new FakeContainerBuilder().Build();
        await testingServiceScope.InsertAsync(container);
        
        var patient = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(patient);
        
        var fakeSampleOne = new FakeSampleForCreationDto()
            .RuleFor(x => x.PatientId, f => patient.Id)
            .Generate();
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
        sampleReturned.Status.Should().Be(SampleStatus.Received().Value);
        sampleReturned.CollectionDate.Should().Be(fakeSampleOne.CollectionDate);
        sampleReturned.ReceivedDate.Should().Be(fakeSampleOne.ReceivedDate);
        sampleReturned.CollectionSite.Should().Be(fakeSampleOne.CollectionSite);
        sampleReturned.ContainerId.Should().Be(container.Id);
        sampleReturned.SampleNumber.Should().NotBeNull();

        sampleCreated.Type.Value.Should().Be(fakeSampleOne.Type);
        sampleCreated.Quantity.Should().Be(fakeSampleOne.Quantity);
        sampleCreated.Status.Should().Be(SampleStatus.Received());
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