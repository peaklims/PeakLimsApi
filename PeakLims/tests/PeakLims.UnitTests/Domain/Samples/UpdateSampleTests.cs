namespace PeakLims.UnitTests.Domain.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class UpdateSampleTests
{
    private readonly Faker _faker;

    public UpdateSampleTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_sample()
    {
        // Arrange
        var fakeSample = new FakeSampleBuilder().Build();
        var updatedSample = new FakeSampleForUpdate().Generate();
        
        // Act
        fakeSample.Update(updatedSample);

        // Assert
        fakeSample.Status.Should().Be(updatedSample.Status);
        fakeSample.Type.Value.Should().Be(updatedSample.Type);
        fakeSample.Quantity.Should().Be(updatedSample.Quantity);
        fakeSample.CollectionDate.Should().Be(updatedSample.CollectionDate);
        fakeSample.ReceivedDate.Should().Be(updatedSample.ReceivedDate);
        fakeSample.CollectionSite.Should().Be(updatedSample.CollectionSite);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakeSample = new FakeSampleBuilder().Build();
        var updatedSample = new FakeSampleForUpdate().Generate();
        fakeSample.DomainEvents.Clear();
        
        // Act
        fakeSample.Update(updatedSample);

        // Assert
        fakeSample.DomainEvents.Count.Should().Be(1);
        fakeSample.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(SampleUpdated));
    }
}