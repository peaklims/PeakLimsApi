namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using System.Threading.Tasks;
using Domain.Panels;
using Domain.TestOrders.Services;
using Domain.Tests.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using PeakLims.Domain.Accessions.Features;
using PeakLims.Domain.Panels.Services;
using Services;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.HealthcareOrganization;
using SharedTestHelpers.Fakes.Panel;
using SharedTestHelpers.Fakes.Patient;
using SharedTestHelpers.Fakes.Test;
using static TestFixture;

public class AddPanelToAccessionCommandPanels : TestBase
{
    [Fact]
    public async Task can_add_panel_to_accession()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var patient = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(patient);
        var org = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(org);
        var test = new FakeTestBuilder().Build().Activate();
        var panel = new FakePanelBuilder().WithTest(test).Build().Activate();
        await testingServiceScope.InsertAsync(panel);

        var accession = new FakeAccessionBuilder().Build();
        accession.SetPatient(patient);
        await testingServiceScope.InsertAsync(accession);

        // Act
        var command = new AddPanelToAccession.Command(accession.Id, panel.Id);
        await testingServiceScope.SendAsync(command);
        var dbAccession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .Include(x => x.TestOrders)
                .ThenInclude(x => x.Test)
            .Include(x => x.TestOrders)
            .ThenInclude(x => x.PanelOrder)
            .ThenInclude(x => x.Panel)
            .FirstOrDefaultAsync(a => a.Id == accession.Id));
        var testOrders = dbAccession.TestOrders;
        var panelOrders = dbAccession.GetPanelOrders();

        // Assert
        testOrders.Count.Should().Be(1);
        testOrders.FirstOrDefault().Test.TestName.Should().Be(panel.Tests.FirstOrDefault().TestName);
        testOrders.FirstOrDefault().PanelOrder.Panel.PanelName.Should().Be(panel.PanelName);
        
        panelOrders.Count.Should().Be(1);
        panelOrders.FirstOrDefault().Panel.PanelName.Should().Be(panel.PanelName);
    }
    
    [Fact]
    public async Task can_add_panel_to_accession_with_existing_test_orders()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        
        var existingText = new FakeTestBuilder().Build().Activate();
        var accession = new FakeAccessionBuilder().Build();
        accession.AddTest(existingText);
        await testingServiceScope.InsertAsync(accession);
        
        var test = new FakeTestBuilder().Build().Activate();
        var panel = new FakePanelBuilder().WithTest(test).Build().Activate();
        await testingServiceScope.InsertAsync(panel);

        // Act
        var command = new AddPanelToAccession.Command(accession.Id, panel.Id);
        await testingServiceScope.SendAsync(command);

        var dbAccession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .Include(x => x.TestOrders)
            .ThenInclude(x => x.Test)
            .Include(x => x.TestOrders)
            .ThenInclude(x => x.PanelOrder)
            .ThenInclude(x => x.Panel)
            .FirstOrDefaultAsync(a => a.Id == accession.Id));
        
        var testOrders = dbAccession.TestOrders;

        // Assert
        testOrders.Count.Should().Be(2);
        testOrders.FirstOrDefault(x => x.Test.Id == existingText.Id).PanelOrder.Should().BeNull();
        testOrders.FirstOrDefault(x => x.Test.Id == test.Id).PanelOrder.Panel.PanelName.Should().Be(panel.PanelName);
    }
}