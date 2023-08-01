namespace PeakLims.IntegrationTests.FeatureTests.Tests;

using PeakLims.Domain.Tests.Dtos;
using PeakLims.SharedTestHelpers.Fakes.Test;
using SharedKernel.Exceptions;
using PeakLims.Domain.Tests.Features;
using FluentAssertions;
using Domain;
using Xunit;
using System.Threading.Tasks;

public class TestListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_test_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTestOne = new FakeTestBuilder().Build();
        var fakeTestTwo = new FakeTestBuilder().Build();
        var queryParameters = new TestParametersDto();

        await testingServiceScope.InsertAsync(fakeTestOne, fakeTestTwo);

        // Act
        var query = new GetTestList.Query(queryParameters);
        var tests = await testingServiceScope.SendAsync(query);

        // Assert
        tests.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadTests);
        var queryParameters = new TestParametersDto();

        // Act
        var command = new GetTestList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}