namespace PeakLims.IntegrationTests.FeatureTests.HipaaAuditLogs;

using PeakLims.SharedTestHelpers.Fakes.HipaaAuditLog;
using PeakLims.Domain.HipaaAuditLogs.Features;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class HipaaAuditLogQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_hipaaauditlog_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var hipaaAuditLogOne = new FakeHipaaAuditLogBuilder().Build();
        await testingServiceScope.InsertAsync(hipaaAuditLogOne);

        // Act
        var query = new GetHipaaAuditLog.Query(hipaaAuditLogOne.Id);
        var hipaaAuditLog = await testingServiceScope.SendAsync(query);

        // Assert
        hipaaAuditLog.Data.Should().Be(hipaaAuditLogOne.Data);
        hipaaAuditLog.ActionBy.Should().Be(hipaaAuditLogOne.ActionBy);
        hipaaAuditLog.Concept.Should().Be(hipaaAuditLogOne.Concept.Value);
        hipaaAuditLog.Action.Should().Be(hipaaAuditLogOne.Action.Value);
    }

    [Fact]
    public async Task get_hipaaauditlog_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetHipaaAuditLog.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadHipaaAuditLogs);

        // Act
        var command = new GetHipaaAuditLog.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}