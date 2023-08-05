namespace PeakLims.UnitTests.Domain.Tests;

using PeakLims.SharedTestHelpers.Fakes.Test;
using PeakLims.Domain.Tests;
using PeakLims.Domain.Tests.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class UpdateTestTests
{
    private readonly Faker _faker;

    public UpdateTestTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_test()
    {
        // Arrange
        var fakeTest = new FakeTestBuilder().Build();
        var updatedTest = new FakeTestForUpdate().Generate();
        
        // Act
        fakeTest.Update(updatedTest);

        // Assert
        fakeTest.TestCode.Should().Be(updatedTest.TestCode);
        fakeTest.TestName.Should().Be(updatedTest.TestName);
        fakeTest.Methodology.Should().Be(updatedTest.Methodology);
        fakeTest.Platform.Should().Be(updatedTest.Platform);
        fakeTest.TurnAroundTime.Should().Be(updatedTest.TurnAroundTime);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakeTest = new FakeTestBuilder().Build();
        var updatedTest = new FakeTestForUpdate().Generate();
        fakeTest.DomainEvents.Clear();
        
        // Act
        fakeTest.Update(updatedTest);

        // Assert
        fakeTest.DomainEvents.Count.Should().Be(1);
        fakeTest.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(TestUpdated));
    }
}