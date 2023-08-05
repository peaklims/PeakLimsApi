namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions.Features;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SharedKernel.Exceptions;
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
        var fakePatientOne = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fakePatientOne);
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne);
        var fakeTest = new FakeTestBuilder().Build().Activate();
        await testingServiceScope.InsertAsync(fakeTest);

        var fakeAccessionOne = new FakeAccessionBuilder()
            // .WithPatient(fakePatientOne)
            // .WithHealthcareOrganization(fakeHealthcareOrganizationOne)
            // .ExcludeTestOrders()
            .Build();
        await testingServiceScope.InsertAsync(fakeAccessionOne);

        // Act
        var command = new AddTestToAccession.Command(fakeAccessionOne.Id, fakeTest.Id);
        await testingServiceScope.SendAsync(command);
        var accession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .Include(x => x.TestOrders)
            .ThenInclude(x => x.Test)
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionOne.Id));
        var testOrders = accession.TestOrders;

        // Assert
        testOrders.Count.Should().Be(1);
        testOrders.FirstOrDefault()!.Test.TestName.Should().Be(fakeTest.TestName);
    }
    [Fact]
    public async Task can_add_test_to_accession_with_existing_test_orders()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePatientOne = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fakePatientOne);
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne);
        var existingTest = new FakeTestBuilder().Build().Activate();
        var fakeTest = new FakeTestBuilder().Build().Activate();
        await testingServiceScope.InsertAsync(fakeTest);

        var fakeAccessionOne = new FakeAccessionBuilder()
            // .WithPatient(fakePatientOne)
            // .WithHealthcareOrganization(fakeHealthcareOrganizationOne)
            .Build()
            .AddTest(existingTest);
        await testingServiceScope.InsertAsync(fakeAccessionOne);

        // Act
        var command = new AddTestToAccession.Command(fakeAccessionOne.Id, fakeTest.Id);
        await testingServiceScope.SendAsync(command);
        var accession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .Include(x => x.TestOrders)
            .ThenInclude(x => x.Test)
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionOne.Id));
        var testOrders = accession.TestOrders;

        // Assert
        testOrders.Count.Should().Be(2);
    }
}