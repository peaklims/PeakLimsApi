namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions.Features;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Domain.Panels.Services;
using Domain.TestOrders.Services;
using Domain.Tests.Services;
using static TestFixture;
using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using Services;
using SharedTestHelpers.Fakes.Panel;
using SharedTestHelpers.Fakes.Test;

public class RemovePanelOrderFromAccessionCommandTests : TestBase
{
    [Fact]
    public async Task can_remove_panel()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePatientOne = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fakePatientOne);
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne);
        var fakeTest = new FakeTestBuilder().Build().Activate();
        var panel = new FakePanelBuilder().WithTest(fakeTest).Build().Activate();

        var fakeAccessionOne = new FakeAccessionBuilder().Build();
        await testingServiceScope.InsertAsync(fakeAccessionOne);
        var panelOrder = fakeAccessionOne.AddPanel(panel);
        await testingServiceScope.InsertAsync(panelOrder);
        var testOrderId = panelOrder.TestOrders.First().Id;

        // Act
        var command = new RemovePanelOrderFromAccession.Command(fakeAccessionOne.Id, panelOrder.Id);
        await testingServiceScope.SendAsync(command);
        var accession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .Include(x => x.TestOrders)
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionOne.Id));
        var testOrderInDb = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == testOrderId));
        var testOrders = accession.TestOrders;

        // Assert
        testOrders.Count.Should().Be(0);
        testOrderInDb.IsDeleted.Should().BeTrue();
    }
}