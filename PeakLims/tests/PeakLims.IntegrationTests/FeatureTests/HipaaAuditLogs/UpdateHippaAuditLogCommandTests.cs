namespace PeakLims.IntegrationTests.FeatureTests.HipaaAuditLogs;

using PeakLims.SharedTestHelpers.Fakes.HipaaAuditLog;
using PeakLims.Domain.HipaaAuditLogs.Dtos;
using PeakLims.Domain.HipaaAuditLogs.Features;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class UpdateHipaaAuditLogCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_hipaaauditlog_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var hipaaAuditLog = new FakeHipaaAuditLogBuilder().Build();
        await testingServiceScope.InsertAsync(hipaaAuditLog);
        var updatedHipaaAuditLogDto = new FakeHipaaAuditLogForUpdateDto().Generate();

        // Act
        var command = new UpdateHipaaAuditLog.Command(hipaaAuditLog.Id, updatedHipaaAuditLogDto);
        await testingServiceScope.SendAsync(command);
        var updatedHipaaAuditLog = await testingServiceScope
            .ExecuteDbContextAsync(db => db.HipaaAuditLogs
                .FirstOrDefaultAsync(h => h.Id == hipaaAuditLog.Id));

        // Assert
        updatedHipaaAuditLog.Data.Should().Be(updatedHipaaAuditLogDto.Data);
        updatedHipaaAuditLog.ActionBy.Should().Be(updatedHipaaAuditLogDto.ActionBy);
        updatedHipaaAuditLog.Concept.Value.Should().Be(updatedHipaaAuditLogDto.Concept);
        updatedHipaaAuditLog.Action.Value.Should().Be(updatedHipaaAuditLogDto.Action);
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdateHipaaAuditLogs);
        var hipaaAuditLogOne = new FakeHipaaAuditLogForUpdateDto();

        // Act
        var command = new UpdateHipaaAuditLog.Command(Guid.NewGuid(), hipaaAuditLogOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}