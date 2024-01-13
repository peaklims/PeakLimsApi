namespace PeakLims.UnitTests.Domain.HipaaAuditLogs;

using PeakLims.SharedTestHelpers.Fakes.HipaaAuditLog;
using PeakLims.Domain.HipaaAuditLogs;
using PeakLims.Domain.HipaaAuditLogs.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using ValidationException = PeakLims.Exceptions.ValidationException;

public class UpdateHipaaAuditLogTests
{
    private readonly Faker _faker;

    public UpdateHipaaAuditLogTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_hipaaAuditLog()
    {
        // Arrange
        var hipaaAuditLog = new FakeHipaaAuditLogBuilder().Build();
        var updatedHipaaAuditLog = new FakeHipaaAuditLogForUpdate().Generate();
        
        // Act
        hipaaAuditLog.Update(updatedHipaaAuditLog);

        // Assert
        hipaaAuditLog.Data.Should().Be(updatedHipaaAuditLog.Data);
        hipaaAuditLog.ActionBy.Should().Be(updatedHipaaAuditLog.ActionBy);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var hipaaAuditLog = new FakeHipaaAuditLogBuilder().Build();
        var updatedHipaaAuditLog = new FakeHipaaAuditLogForUpdate().Generate();
        hipaaAuditLog.DomainEvents.Clear();
        
        // Act
        hipaaAuditLog.Update(updatedHipaaAuditLog);

        // Assert
        hipaaAuditLog.DomainEvents.Count.Should().Be(1);
        hipaaAuditLog.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(HipaaAuditLogUpdated));
    }
}