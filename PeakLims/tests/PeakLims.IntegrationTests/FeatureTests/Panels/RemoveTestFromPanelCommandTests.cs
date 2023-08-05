namespace PeakLims.IntegrationTests.FeatureTests.Panels;

using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.Domain.Panels.Features;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using System.Threading.Tasks;
using Domain.Panels.Services;
using Domain.TestOrders.Services;
using Domain.Tests.Services;
using Services;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.HealthcareOrganization;
using SharedTestHelpers.Fakes.Patient;
using SharedTestHelpers.Fakes.Test;
using Xunit;
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
            .Include(x => x.Tests)
            .FirstOrDefaultAsync(p => p.Id == panel.Id));

        // Assert 
        panelFromDb.Tests.Count.Should().Be(0);
    }
    
    [Fact]
    public async Task can_not_remove_test_to_panel_if_panel_is_actively_used()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var test = new FakeTestBuilder()
            .Build()
            .Activate();
        var secondTest = new FakeTestBuilder()
            .Build()
            .Activate();
        await testingServiceScope.InsertAsync(secondTest);
        var panel = new FakePanelBuilder().WithTest(test)
            .Build()
            .Activate();

        var fakePatientOne = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fakePatientOne);
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne);
        var accession = new FakeAccessionBuilder()
            .WithPanel(panel)
            .Build();
        await testingServiceScope.InsertAsync(accession);

        // Act
        var command = new RemoveTestFromPanel.Command(panel.Id, secondTest.Id);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}