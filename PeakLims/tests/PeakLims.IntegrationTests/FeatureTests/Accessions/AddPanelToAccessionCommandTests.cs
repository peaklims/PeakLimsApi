namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using System.Threading.Tasks;
using Domain.Panels;
using Domain.TestOrders.Services;
using Domain.Tests.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
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

        var fakeAccessionOne = new FakeAccessionBuilder()
            // .ExcludeTestOrders()
            .Build();
        await testingServiceScope.InsertAsync(fakeAccessionOne);

        // Act
        var command = new AddPanelToAccession.Command(fakeAccessionOne.Id, fakePanel.Id);
        await testingServiceScope.SendAsync(command);
        var accession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .Include(x => x.TestOrders)
            .ThenInclude(x => x.Test)
            .ThenInclude(x => x.Panels)
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionOne.Id));
        var testOrders = accession.TestOrders;

        // Assert
        testOrders.Count.Should().Be(1);
        testOrders.FirstOrDefault().Test.TestName.Should().Be(fakePanel.Tests.FirstOrDefault().TestName);
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

        var fakeAccessionOne = new FakeAccessionBuilder()
            // .WithPatient(fakePatientOne)
            // .WithHealthcareOrganization(fakeHealthcareOrganizationOne)
            .Build()
            .AddTest(existingText);
        await testingServiceScope.InsertAsync(fakeAccessionOne);

        // Act
        var command = new AddPanelToAccession.Command(fakeAccessionOne.Id, fakePanel.Id);
        await testingServiceScope.SendAsync(command);
        var accession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .Include(x => x.TestOrders)
            .ThenInclude(x => x.Test)
            .ThenInclude(x => x.Panels)
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionOne.Id));
        var testOrders = accession.TestOrders;

        // Assert
        testOrders.Count.Should().Be(2);
    }
}