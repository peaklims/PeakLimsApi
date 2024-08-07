namespace PeakLims.IntegrationTests.FeatureTests.HipaaAuditLogs;

using PeakLims.Domain.HipaaAuditLogs.Dtos;
using PeakLims.SharedTestHelpers.Fakes.HipaaAuditLog;
using PeakLims.Domain.HipaaAuditLogs.Features;
using Domain;
using System.Threading.Tasks;

public class HipaaAuditLogListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_hipaaauditlog_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var hipaaAuditLogOne = new FakeHipaaAuditLogBuilder().Build();
        var hipaaAuditLogTwo = new FakeHipaaAuditLogBuilder().Build();
        var queryParameters = new HipaaAuditLogParametersDto();

        await testingServiceScope.InsertAsync(hipaaAuditLogOne, hipaaAuditLogTwo);

        // Act
        var query = new GetHipaaAuditLogList.Query(queryParameters);
        var hipaaAuditLogs = await testingServiceScope.SendAsync(query);

        // Assert
        hipaaAuditLogs.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadHipaaAuditLogs);
        var queryParameters = new HipaaAuditLogParametersDto();

        // Act
        var command = new GetHipaaAuditLogList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}