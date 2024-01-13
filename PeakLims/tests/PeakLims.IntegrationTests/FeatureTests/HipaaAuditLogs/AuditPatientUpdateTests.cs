namespace PeakLims.IntegrationTests.FeatureTests.HipaaAuditLogs;

using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using PeakLims.Domain.AuditLogActions;
using PeakLims.Domain.AuditLogConcepts;
using PeakLims.Domain.HipaaAuditLogs.Models;
using PeakLims.Domain.Patients.Features;
using PeakLims.SharedTestHelpers.Fakes.Patient;

public class AuditPatientUpdateTests : TestBase
{
    [Fact]
    public async Task can_audit_patient_update()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePatientOne = new FakePatientBuilder().Build();
        var updatedPatientDto = new FakePatientForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakePatientOne);

        var patient = await testingServiceScope.ExecuteDbContextAsync(db => db.Patients
            .FirstOrDefaultAsync(p => p.Id == fakePatientOne.Id));

        // Act
        var command = new UpdatePatient.Command(patient.Id, updatedPatientDto);
        await testingServiceScope.SendAsync(command);
        var updatedPatient = await testingServiceScope.ExecuteDbContextAsync(db => db.Patients.FirstOrDefaultAsync(p => p.Id == patient.Id));
        var auditLogs = await testingServiceScope.ExecuteDbContextAsync(db => db.HipaaAuditLogs
            .Where(p => p.Identifier == updatedPatient.Id)
            .ToListAsync());

        // Assert
        var auditLog = auditLogs.FirstOrDefault(x => x.Action == AuditLogAction.Updated());
        auditLog.Should().NotBeNull();
        auditLog.Identifier.Should().Be(updatedPatient.Id);
        auditLog.Concept.Should().Be(AuditLogConcept.Patient());
        // auditLog.ActionBy.Should().NotBeNullOrEmpty();
        auditLog.Action.Should().Be(AuditLogAction.Updated());
        auditLog.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, 1.Seconds());
        
        var auditData = JsonSerializer.Deserialize<PatientAuditLogEntry>(auditLog.Data);
        auditData.Id.Should().Be(updatedPatient.Id);
        auditData.FirstName.Should().Be(updatedPatient.FirstName);
        auditData.LastName.Should().Be(updatedPatient.LastName);
        auditData.DateOfBirth.Should().Be(updatedPatient.Lifespan.DateOfBirth);
        auditData.Sex.Should().Be(updatedPatient.Sex);
        auditData.Race.Should().Be(updatedPatient.Race);
        auditData.Ethnicity.Should().Be(updatedPatient.Ethnicity);
        auditData.InternalId.Should().NotBeNull();
    }
}