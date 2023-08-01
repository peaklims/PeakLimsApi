namespace PeakLims.IntegrationTests.FeatureTests.Panels;

using PeakLims.Domain.Panels.Dtos;
using PeakLims.SharedTestHelpers.Fakes.Panel;
using SharedKernel.Exceptions;
using PeakLims.Domain.Panels.Features;
using FluentAssertions;
using Domain;
using Xunit;
using System.Threading.Tasks;

public class PanelListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_panel_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePanelOne = new FakePanelBuilder().Build();
        var fakePanelTwo = new FakePanelBuilder().Build();
        var queryParameters = new PanelParametersDto();

        await testingServiceScope.InsertAsync(fakePanelOne, fakePanelTwo);

        // Act
        var query = new GetPanelList.Query(queryParameters);
        var panels = await testingServiceScope.SendAsync(query);

        // Assert
        panels.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadPanels);
        var queryParameters = new PanelParametersDto();

        // Act
        var command = new GetPanelList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}