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

public class AuditPatientCreationTests : TestBase
{
    [Fact]
    public async Task can_audit_new_patient()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var patient = new FakePatientForCreationDto().Generate();

        // Act
        var command = new AddPatient.Command(patient);
        var patientResponse = await testingServiceScope.SendAsync(command);
        var auditLog = await testingServiceScope.ExecuteDbContextAsync(db => db.HipaaAuditLogs
            .FirstOrDefaultAsync(p => p.Identifier == patientResponse.Id));

        // Assert
        auditLog.Should().NotBeNull();
        auditLog.Identifier.Should().Be(patientResponse.Id);
        auditLog.Concept.Should().Be(AuditLogConcept.Patient());
        // auditLog.ActionBy.Should().NotBeNullOrEmpty();
        auditLog.Action.Should().Be(AuditLogAction.Added());
        auditLog.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, 1.Seconds());
        
        var auditData = JsonSerializer.Deserialize<PatientAuditLogEntry>(auditLog.Data);
        auditData.Id.Should().Be(patientResponse.Id);
        auditData.FirstName.Should().Be(patientResponse.FirstName);
        auditData.LastName.Should().Be(patientResponse.LastName);
        auditData.DateOfBirth.Should().Be(patientResponse.DateOfBirth);
        auditData.Sex.Should().Be(patientResponse.Sex);
        auditData.Race.Should().Be(patientResponse.Race);
        auditData.Ethnicity.Should().Be(patientResponse.Ethnicity);
        auditData.InternalId.Should().NotBeNull();
    }
}