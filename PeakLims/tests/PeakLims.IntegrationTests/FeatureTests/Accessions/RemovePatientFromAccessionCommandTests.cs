namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using PeakLims.SharedTestHelpers.Fakes.Accession;
using PeakLims.Domain.Accessions.Features;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

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

public class RemovePatientFromAccessionCommandTests : TestBase
{
    [Fact]
    public async Task can_remove_patient()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePatientOne = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(fakePatientOne);

        var fakeAccessionOne = new FakeAccessionBuilder()
            .Build()
            .SetPatient(fakePatientOne);
        await testingServiceScope.InsertAsync(fakeAccessionOne);

        // Act
        var command = new RemoveAccessionPatient.Command(fakeAccessionOne.Id);
        await testingServiceScope.SendAsync(command);
        var accession = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .Include(x => x.Patient)
            .FirstOrDefaultAsync(a => a.Id == fakeAccessionOne.Id));

        // Assert
        accession.Patient.Should().BeNull();
    }
}