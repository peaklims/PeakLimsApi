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
        var fakePatientOne = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fakePatientOne);
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne);
        var fakeTest = new FakeTestBuilder().Build().Activate();
        var fakePanel = new FakePanelBuilder().WithTest(fakeTest).Build().Activate();
        await testingServiceScope.InsertAsync(fakePanel);

        var fakeAccessionOne = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(fakeAccessionOne);

        // Act
        var command = new AddPanelToAccession.Command(fakeAccessionOne.Id, fakePanel.Id);
        await testingServiceScope.SendAsync(command);
        var panelOrders = await testingServiceScope.ExecuteDbContextAsync(db => db.PanelOrders
            .Include(x => x.Accession)
            .Include(x => x.Panel)
            .Include(x => x.TestOrders)
                .ThenInclude(x => x.Test)
            .FirstOrDefaultAsync(a => a.Accession.Id == fakeAccessionOne.Id));
        var testOrders = panelOrders.TestOrders;

        // Assert
        testOrders.Count.Should().Be(1);
        testOrders.FirstOrDefault().Test.TestName.Should().Be(fakePanel.Tests.FirstOrDefault().TestName);
        testOrders.FirstOrDefault().PanelOrder.Panel.PanelName.Should().Be(fakePanel.PanelName);
    }
    
    [Fact]
    public async Task can_add_panel_to_accession_with_existing_test_orders()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePatientOne = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fakePatientOne);
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne);
        var existingText = new FakeTestBuilder().Build().Activate();
        var fakeTest = new FakeTestBuilder().Build().Activate();
        var fakePanel = new FakePanelBuilder().WithTest(fakeTest).Build().Activate();
        await testingServiceScope.InsertAsync(fakePanel);

        var accession = new FakeAccessionBuilder().Build();
        accession.AddTest(existingText);
        await testingServiceScope.InsertAsync(accession);

        // Act
        var command = new AddPanelToAccession.Command(accession.Id, fakePanel.Id);
        await testingServiceScope.SendAsync(command);
        var panelOrders = await testingServiceScope.ExecuteDbContextAsync(db => db.PanelOrders
            .Include(x => x.Accession)
            .Include(x => x.Panel)
            .Include(x => x.TestOrders)
            .ThenInclude(x => x.Test)
            .Where(a => a.Accession.Id == accession.Id)
            .ToListAsync());
        var dbTestOrders = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .Include(x => x.Accession)
            .Include(x => x.Test)
            .Where(a => a.Accession.Id == accession.Id)
            .ToListAsync());
        
        var testOrders = panelOrders.SelectMany(x => x.TestOrders)
            .Concat(dbTestOrders)
            .ToList();

        // Assert
        testOrders.Count.Should().Be(2);
        testOrders.FirstOrDefault(x => x.Test.Id == existingText.Id).PanelOrder.Should().BeNull();
        testOrders.FirstOrDefault(x => x.Test.Id == fakeTest.Id).PanelOrder.Panel.PanelName.Should().Be(fakePanel.PanelName);
    }
}