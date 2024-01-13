namespace PeakLims.FunctionalTests.FunctionalTests.HipaaAuditLogs;

using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using PeakLims.Domain.AuditLogActions;
using PeakLims.Domain.AuditLogConcepts;
using PeakLims.Domain.HipaaAuditLogs.Models;
using PeakLims.FunctionalTests;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.SharedTestHelpers.Fakes.Patient;

public class AuditPatientUpdateTests : TestBase
{
    [Fact]
    public async Task can_audit_patient_update()
    {
        // Arrange
        var callingUser = await AddNewSuperAdmin();
        FactoryClient.AddAuth(callingUser.Identifier);
        var patient = new FakePatientBuilder().Build();
        var updatedPatientDto = new FakePatientForUpdateDto().Generate();
        await InsertAsync(patient);

        // Act
        var route = ApiRoutes.Patients.Put(patient.Id);
        await FactoryClient.PutJsonRequestAsync(route, updatedPatientDto);
        var auditLog = await ExecuteDbContextAsync(db => db.HipaaAuditLogs
            .FirstOrDefaultAsync(p => p.ActionBy == callingUser.Identifier));
        
        // Assert
        auditLog.Should().NotBeNull();
        auditLog.Identifier.Should().NotBeEmpty();
        auditLog.Concept.Should().Be(AuditLogConcept.Patient());
        auditLog.ActionBy.Should().Be(callingUser.Identifier);
        auditLog.Action.Should().Be(AuditLogAction.Updated());
        auditLog.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, 1.Seconds());
        
        var auditData = JsonSerializer.Deserialize<PatientAuditLogEntry>(auditLog.Data);
        auditData.Id.Should().Be(patient.Id);
        auditData.FirstName.Should().Be(updatedPatientDto.FirstName);
        auditData.LastName.Should().Be(updatedPatientDto.LastName);
        auditData.DateOfBirth.Should().Be(updatedPatientDto.DateOfBirth);
        auditData.Sex.Should().Be(updatedPatientDto.Sex);
        auditData.Race.Should().Be(updatedPatientDto.Race);
        auditData.Ethnicity.Should().Be(updatedPatientDto.Ethnicity);
        auditData.InternalId.Should().NotBeNull();
    }
}