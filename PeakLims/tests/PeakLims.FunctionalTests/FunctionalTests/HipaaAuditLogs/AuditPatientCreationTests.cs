namespace PeakLims.FunctionalTests.FunctionalTests.HipaaAuditLogs;

using System.Net.Http.Json;
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

public class AuditPatientCreationTests : TestBase
{
    [Fact]
    public async Task can_audit_new_patient()
    {
        // Arrange
        var callingUser = await AddNewSuperAdmin();
        FactoryClient.AddAuth(callingUser.Identifier);
        var patientForCreationDto = new FakePatientForCreationDto().Generate();

        // Act
        var route = ApiRoutes.Patients.Create;
        var responseContent = await FactoryClient.PostJsonRequestAsync(route, patientForCreationDto);
        var patientResponse = await responseContent.Content.ReadFromJsonAsync<PatientAuditLogEntry>();
        var auditLog = await ExecuteDbContextAsync(db => db.HipaaAuditLogs
            .FirstOrDefaultAsync(p => p.ActionBy == callingUser.Identifier));
        
        // Assert
        auditLog.Should().NotBeNull();
        auditLog.Identifier.Should().NotBeEmpty();
        auditLog.Concept.Should().Be(AuditLogConcept.Patient());
        auditLog.ActionBy.Should().Be(callingUser.Identifier);
        auditLog.Action.Should().Be(AuditLogAction.Added());
        auditLog.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, 1.Seconds());
        
        var auditData = JsonSerializer.Deserialize<PatientAuditLogEntry>(auditLog.Data);
        auditData.Id.Should().Be(patientResponse.Id);
        auditData.FirstName.Should().Be(patientForCreationDto.FirstName);
        auditData.LastName.Should().Be(patientForCreationDto.LastName);
        auditData.DateOfBirth.Should().Be(patientForCreationDto.DateOfBirth);
        auditData.Sex.Should().Be(patientForCreationDto.Sex);
        auditData.Race.Should().Be(patientForCreationDto.Race);
        auditData.Ethnicity.Should().Be(patientForCreationDto.Ethnicity);
        auditData.InternalId.Should().NotBeNull();
    }
}