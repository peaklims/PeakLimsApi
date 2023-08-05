namespace PeakLims.UnitTests.Domain.Tests;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.Tests.DomainEvents;
using PeakLims.Domain.TestStatuses;
using PeakLims.SharedTestHelpers.Fakes.Test;
using Xunit;

public class TestStateChangeTests
{
    private readonly Faker _faker;

    public TestStateChangeTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_activate_test()
    {
        // Arrange
        var fakeTest = new FakeTestBuilder().Build();
        fakeTest.DomainEvents.Clear();
        
        // Act
        fakeTest.Activate();

        // Assert
        fakeTest.Status.Should().Be(TestStatus.Active());
        fakeTest.DomainEvents.Count.Should().Be(1);
        fakeTest.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(TestUpdated));
    }
    
    [Fact]
    public void can_deactivate_test()
    {
        // Arrange
        var fakeTest = new FakeTestBuilder().Build();
        fakeTest.DomainEvents.Clear();
        
        // Act
        fakeTest.Deactivate();

        // Assert
        fakeTest.Status.Should().Be(TestStatus.Inactive());
        fakeTest.DomainEvents.Count.Should().Be(1);
        fakeTest.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(TestUpdated));
    }
}