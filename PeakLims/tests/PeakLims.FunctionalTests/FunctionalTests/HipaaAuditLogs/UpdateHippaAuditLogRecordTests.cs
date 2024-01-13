namespace PeakLims.FunctionalTests.FunctionalTests.HipaaAuditLogs;

using System.Net;
using System.Threading.Tasks;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.SharedTestHelpers.Fakes.HipaaAuditLog;

public class UpdateHipaaAuditLogRecordTests : TestBase
{
    [Fact]
    public async Task put_hipaaauditlog_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var hipaaAuditLog = new FakeHipaaAuditLogBuilder().Build();
        var updatedHipaaAuditLogDto = new FakeHipaaAuditLogForUpdateDto().Generate();

        var callingUser = await AddNewSuperAdmin();
        FactoryClient.AddAuth(callingUser.Identifier);
        await InsertAsync(hipaaAuditLog);

        // Act
        var route = ApiRoutes.HipaaAuditLogs.Put(hipaaAuditLog.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedHipaaAuditLogDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}