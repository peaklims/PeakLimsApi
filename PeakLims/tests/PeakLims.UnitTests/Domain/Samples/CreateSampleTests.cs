namespace PeakLims.UnitTests.Domain.Samples;

using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class CreateSampleTests
{
    private readonly Faker _faker;

    public CreateSampleTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_sample()
    {
        // Arrange
        var sampleToCreate = new FakeSampleForCreation().Generate();
        
        // Act
        var fakeSample = Sample.Create(sampleToCreate);

        // Assert
        fakeSample.Status.Should().Be(sampleToCreate.Status);
        fakeSample.Type.Value.Should().Be(sampleToCreate.Type);
        fakeSample.Quantity.Should().Be(sampleToCreate.Quantity);
        fakeSample.CollectionDate.Should().Be(sampleToCreate.CollectionDate);
        fakeSample.ReceivedDate.Should().Be(sampleToCreate.ReceivedDate);
        fakeSample.CollectionSite.Should().Be(sampleToCreate.CollectionSite);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var sampleToCreate = new FakeSampleForCreation().Generate();
        
        // Act
        var fakeSample = Sample.Create(sampleToCreate);

        // Assert
        fakeSample.DomainEvents.Count.Should().Be(1);
        fakeSample.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(SampleCreated));
    }
}