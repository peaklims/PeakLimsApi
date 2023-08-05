namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions.Features;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SharedKernel.Exceptions;
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

public class RemovePanelFromAccessionCommandTests : TestBase
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
        var fakePanel = new FakePanelBuilder().WithTest(fakeTest).Build().Activate();

        var fakeAccessionOne = new FakeAccessionBuilder()
            // .WithPatient(fakePatientOne)
            // .WithHealthcareOrganization(fakeHealthcareOrganizationOne)
            .Build()
            .AddPanel(fakePanel);
        await testingServiceScope.InsertAsync(fakeAccessionOne);

        var testOrder = fakeAccessionOne.TestOrders.First();

        // Act
        var command = new RemovePanelFromAccession.Command(fakeAccessionOne.Id, fakePanel.Id);
        await testingServiceScope.SendAsync(command);
        var accession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .Include(x => x.TestOrders)
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionOne.Id));
        var testOrderInDb = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == testOrder.Id));
        var testOrders = accession.TestOrders;

        // Assert
        testOrders.Count.Should().Be(0);
        testOrderInDb.IsDeleted.Should().BeTrue();
    }
}