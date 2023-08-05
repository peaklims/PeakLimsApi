namespace PeakLims.UnitTests.Domain.Tests;

using PeakLims.SharedTestHelpers.Fakes.Test;
using PeakLims.Domain.Tests;
using PeakLims.Domain.Tests.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using PeakLims.Domain.TestStatuses;
using Xunit;

public class CreateTestTests
{
    private readonly Faker _faker;

    public CreateTestTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_test()
    {
        // Arrange
        var testToCreate = new FakeTestForCreation().Generate();
        
        // Act
        var fakeTest = Test.Create(testToCreate);

        // Assert
        fakeTest.TestCode.Should().Be(testToCreate.TestCode);
        fakeTest.TestName.Should().Be(testToCreate.TestName);
        fakeTest.Methodology.Should().Be(testToCreate.Methodology);
        fakeTest.Platform.Should().Be(testToCreate.Platform);
        fakeTest.TurnAroundTime.Should().Be(testToCreate.TurnAroundTime);
        fakeTest.Status.Should().Be(TestStatus.Draft());
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var testToCreate = new FakeTestForCreation().Generate();
        
        // Act
        var fakeTest = Test.Create(testToCreate);

        // Assert
        fakeTest.DomainEvents.Count.Should().Be(1);
        fakeTest.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(TestCreated));
    }
}