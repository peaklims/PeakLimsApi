namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions.Features;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Domain.Tests.Services;
using static TestFixture;
using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using Services;
using SharedTestHelpers.Fakes.Test;

public class AddTestToAccessionCommandTests : TestBase
{
    [Fact]
    public async Task can_add_test_to_accession()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var patient = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(patient);
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne);
        var fakeTest = new FakeTestBuilder().Build().Activate();
        await testingServiceScope.InsertAsync(fakeTest);

        var accession = new FakeAccessionBuilder().Build();
        accession.SetPatient(patient);
        await testingServiceScope.InsertAsync(accession);

        // Act
        var command = new AddTestToAccession.Command(accession.Id, fakeTest.Id);
        await testingServiceScope.SendAsync(command);
        var dbaccession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .Include(x => x.TestOrders)
            .ThenInclude(x => x.Test)
            .FirstOrDefaultAsync(a => a.Id == accession.Id));
        var testOrders = dbaccession.TestOrders;

        // Assert
        testOrders.Count.Should().Be(1);
        testOrders.FirstOrDefault()!.Test.TestName.Should().Be(fakeTest.TestName);
    }
    [Fact]
    public async Task can_add_test_to_accession_with_existing_test_orders()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var patient = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(patient);
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne);
        var existingTest = new FakeTestBuilder().Build().Activate();
        var fakeTest = new FakeTestBuilder().Build().Activate();
        await testingServiceScope.InsertAsync(fakeTest);

        var accession = new FakeAccessionBuilder().Build();
        accession.AddTest(existingTest);
        accession.SetPatient(patient);
        await testingServiceScope.InsertAsync(accession);

        // Act
        var command = new AddTestToAccession.Command(accession.Id, fakeTest.Id);
        await testingServiceScope.SendAsync(command);
        var dbaccession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .Include(x => x.TestOrders)
            .ThenInclude(x => x.Test)
            .FirstOrDefaultAsync(a => a.Id == accession.Id));
        var testOrders = dbaccession.TestOrders;

        // Assert
        testOrders.Count.Should().Be(2);
    }
}