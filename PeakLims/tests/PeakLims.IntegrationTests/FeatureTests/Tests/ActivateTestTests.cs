namespace PeakLims.IntegrationTests.FeatureTests.Tests;

using System.Threading.Tasks;
using Domain.Tests.Features;
using Domain.Tests.Services;
using Domain.TestStatuses;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SharedTestHelpers.Fakes.Test;
using Xunit;
using static TestFixture;

public class ActivateTestTests : TestBase
{
    [Fact]
    public async Task can_activate_test()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTest = new FakeTestBuilder().Build();
        await testingServiceScope.InsertAsync(fakeTest);

        // Act
        var command = new ActivateTest.Command(fakeTest.Id);
        await testingServiceScope.SendAsync(command);
        var updatedTest = await testingServiceScope.ExecuteDbContextAsync(db => db.Tests.FirstOrDefaultAsync(a => a.Id == fakeTest.Id));

        // Assert
        updatedTest.Status.Should().Be(TestStatus.Active());
    }
}