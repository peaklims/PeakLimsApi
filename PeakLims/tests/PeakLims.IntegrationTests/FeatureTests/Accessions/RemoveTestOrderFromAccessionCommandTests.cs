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

public class RemoveTestOrderFromAccessionCommandTests : TestBase
{
    [Fact]
    public async Task can_remove_testorder()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePatientOne = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fakePatientOne);
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne);

        var test = new FakeTestBuilder().Build().Activate();
        await testingServiceScope.InsertAsync(test);
        
        var fakeAccessionOne = new FakeAccessionBuilder()
            .WithTest(test)
            .Build();
        await testingServiceScope.InsertAsync(fakeAccessionOne);

        var testOrder = fakeAccessionOne.TestOrders.First();

        // Act
        var command = new RemoveTestOrderFromAccession.Command(fakeAccessionOne.Id, testOrder.Id);
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