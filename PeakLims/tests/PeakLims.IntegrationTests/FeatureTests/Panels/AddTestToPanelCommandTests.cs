namespace PeakLims.IntegrationTests.FeatureTests.Panels;

using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.Domain.Panels.Features;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Domain.Panels.Services;
using Domain.TestOrders.Services;
using Domain.Tests.Services;
using Exceptions;
using Services;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.HealthcareOrganization;
using SharedTestHelpers.Fakes.Patient;
using SharedTestHelpers.Fakes.Test;

using static TestFixture;

public class AddTestToPanelCommandTests : TestBase
{
    [Fact]
    public async Task can_add_test_to_panel()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var test = new FakeTestBuilder().Build().Activate();
        await testingServiceScope.InsertAsync(test);
        var panel = new FakePanelBuilder().Build();
        await testingServiceScope.InsertAsync(panel);

        // Act
        var command = new AddTestToPanel.Command(panel.Id, test.Id, 1);
        await testingServiceScope.SendAsync(command);
        var panelFromDb = await testingServiceScope.ExecuteDbContextAsync(db => db.Panels
            .Include(x => x.TestAssignments)
            .ThenInclude(x => x.Test)
            .FirstOrDefaultAsync(p => p.Id == panel.Id));

        // Assert
        panelFromDb.TestAssignments.Count.Should().Be(1);
        panelFromDb.TestAssignments.First().Test.TestName.Should().Be(test.TestName);
    }

    [Fact]
    public async Task can_add_test_to_panel_with_multiple_entries()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var test = new FakeTestBuilder().Build().Activate();
        await testingServiceScope.InsertAsync(test);
        var panel = new FakePanelBuilder().Build();
        await testingServiceScope.InsertAsync(panel);

        // Act
        var command = new AddTestToPanel.Command(panel.Id, test.Id, 2);
        await testingServiceScope.SendAsync(command);
        var panelFromDb = await testingServiceScope.ExecuteDbContextAsync(db => db.Panels
        .Include(x => x.TestAssignments)
        .ThenInclude(x => x.Test)
        .FirstOrDefaultAsync(p => p.Id == panel.Id));

        // Assert
        panelFromDb.TestAssignments.Count.Should().Be(1);
        panelFromDb.TestAssignments.First().Test.TestName.Should().Be(test.TestName);
        panelFromDb.TestAssignments.First().TestCount.Should().Be(2);
    }
    
    [Fact]
    public async Task can_not_add_test_to_panel_if_panel_is_actively_used()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var test = new FakeTestBuilder().Build().Activate();
        var secondTest = new FakeTestBuilder().Build().Activate();
        await testingServiceScope.InsertAsync(secondTest);
        var panel = new FakePanelBuilder().WithTest(test).Build().Activate();
        
        var accession = new FakeAccessionBuilder().Build();
        accession.AddPanel(panel);
        await testingServiceScope.InsertAsync(accession);

        // Act
        var command = new AddTestToPanel.Command(panel.Id, secondTest.Id, 1);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}