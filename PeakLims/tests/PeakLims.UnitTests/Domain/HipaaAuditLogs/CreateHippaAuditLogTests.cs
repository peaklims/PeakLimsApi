namespace PeakLims.UnitTests.Domain.HipaaAuditLogs;

using PeakLims.SharedTestHelpers.Fakes.HipaaAuditLog;
using PeakLims.Domain.HipaaAuditLogs;
using PeakLims.Domain.HipaaAuditLogs.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using ValidationException = PeakLims.Exceptions.ValidationException;

public class CreateHipaaAuditLogTests
{
    private readonly Faker _faker;

    public CreateHipaaAuditLogTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_hipaaAuditLog()
    {
        // Arrange
        var hipaaAuditLogToCreate = new FakeHipaaAuditLogForCreation().Generate();
        
        // Act
        var hipaaAuditLog = HipaaAuditLog.Create(hipaaAuditLogToCreate);

        // Assert
        hipaaAuditLog.Data.Should().Be(hipaaAuditLogToCreate.Data);
        hipaaAuditLog.ActionBy.Should().Be(hipaaAuditLogToCreate.ActionBy);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var hipaaAuditLogToCreate = new FakeHipaaAuditLogForCreation().Generate();
        
        // Act
        var hipaaAuditLog = HipaaAuditLog.Create(hipaaAuditLogToCreate);

        // Assert
        hipaaAuditLog.DomainEvents.Count.Should().Be(1);
        hipaaAuditLog.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(HipaaAuditLogCreated));
    }
}