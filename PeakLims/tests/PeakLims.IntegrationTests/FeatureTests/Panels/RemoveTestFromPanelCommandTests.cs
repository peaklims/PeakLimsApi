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

public class RemoveTestFromPanelCommandTests : TestBase
{
    [Fact]
    public async Task can_remove_test_from_panel()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var test = new FakeTestBuilder().Build().Activate();
        var panel = new FakePanelBuilder().WithTest(test).Build();
        await testingServiceScope.InsertAsync(panel);

        // Act
        var command = new RemoveTestFromPanel.Command(panel.Id, test.Id);
        await testingServiceScope.SendAsync(command);
        var panelFromDb = await testingServiceScope.ExecuteDbContextAsync(db => db.Panels
            .Include(x => x.TestAssignments)
            .ThenInclude(x => x.Test)
            .FirstOrDefaultAsync(p => p.Id == panel.Id));

        // Assert 
        panelFromDb.TestAssignments.Count.Should().Be(0);
    }
    
    [Fact]
    public async Task can_not_remove_test_to_panel_if_panel_is_actively_used()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var test = new FakeTestBuilder()
            .Build()
            .Activate();
        var panel = new FakePanelBuilder()
            .WithTest(test)
            .Build()
            .Activate();
        
        var accession = new FakeAccessionBuilder().Build();
        accession.AddPanel(panel);
        await testingServiceScope.InsertAsync(accession);

        // Act
        var command = new RemoveTestFromPanel.Command(panel.Id, test.Id);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}