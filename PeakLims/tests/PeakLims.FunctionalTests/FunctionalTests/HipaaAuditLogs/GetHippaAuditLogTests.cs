namespace PeakLims.FunctionalTests.FunctionalTests.HipaaAuditLogs;

using System.Net;
using System.Threading.Tasks;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.SharedTestHelpers.Fakes.HipaaAuditLog;

public class GetHipaaAuditLogTests : TestBase
{
    [Fact]
    public async Task get_hipaaauditlog_returns_success_when_entity_exists_using_valid_auth_credentials()
    {
        // Arrange
        var hipaaAuditLog = new FakeHipaaAuditLogBuilder().Build();

        var callingUser = await AddNewSuperAdmin();
        FactoryClient.AddAuth(callingUser.Identifier);
        await InsertAsync(hipaaAuditLog);

        // Act
        var route = ApiRoutes.HipaaAuditLogs.GetRecord(hipaaAuditLog.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
            
    [Fact]
    public async Task get_hipaaauditlog_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var hipaaAuditLog = new FakeHipaaAuditLogBuilder().Build();

        // Act
        var route = ApiRoutes.HipaaAuditLogs.GetRecord(hipaaAuditLog.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task get_hipaaauditlog_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var hipaaAuditLog = new FakeHipaaAuditLogBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.HipaaAuditLogs.GetRecord(hipaaAuditLog.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}