namespace PeakLims.UnitTests.Domain.TestOrders;

using Bogus;
using FluentAssertions;
using Moq;
using PeakLims.Domain.TestOrders;
using PeakLims.Services;
using PeakLims.SharedTestHelpers.Fakes.Test;
using SharedTestHelpers.Fakes.Container;
using SharedTestHelpers.Fakes.Sample;
using Xunit;

public class ManageTestOrderSampleTests
{
    private readonly Faker _faker;

    public ManageTestOrderSampleTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_manage_sample()
    {
        // Arrange
        var container = new FakeContainerBuilder().Build();
        var sample = new FakeSampleBuilder().WithValidContainer(container).Build();
        var test = new FakeTestBuilder().Build().Activate();
        var fakeTestOrder = TestOrder.Create(test);
        
        // Act -- add
        fakeTestOrder.SetSample(sample);

        // Assert -- add
        fakeTestOrder.Sample.Id.Should().Be(sample.Id);
        fakeTestOrder.Sample.Should().Be(sample);
        
        // Act -- remove
        fakeTestOrder.RemoveSample();

        // Assert -- remove
        fakeTestOrder.Sample.Should().BeNull();
        fakeTestOrder.Sample.Should().BeNull();
    }

    [Fact]
    public void can_not_update_sample_when_processing_accession()
    {
        // Arrange
        var container = new FakeContainerBuilder().Build();
        var sample = new FakeSampleBuilder().WithValidContainer(container).Build();
        var test = new FakeTestBuilder().Build().Activate();
        var testOrder = TestOrder.Create(test);
        testOrder.SetSample(sample);
        testOrder.SetStatusToReadyForTesting();

        var anotherSample = new FakeSampleBuilder().WithValidContainer(container).Build();
        
        // Act
        var actAdd = () => testOrder.SetSample(anotherSample);
        var actRemove = () => testOrder.RemoveSample();

        // Assert
        actAdd.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage($"The assigned sample can not be updated once a test order has started processing.");
        actRemove.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage($"The assigned sample can not be updated once a test order has started processing.");
    }

    [Fact]
    public void must_use_valid_sample()
    {
        // Arrange
        var test = new FakeTestBuilder().Build().Activate();
        var testOrder = TestOrder.Create(test);
        
        // Act
        var actAdd = () => testOrder.SetSample(null);

        // Assert
        actAdd.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage($"A valid sample must be provided.");
    }
}