namespace PeakLims.IntegrationTests.FeatureTests.Accessions;

using System.Threading.Tasks;
using Bogus;
using Domain.Accessions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using PeakLims.Domain.Accessions.Features;
using PeakLims.Domain.Tests.Services;
using PeakLims.SharedTestHelpers.Fakes.Patient;
using PeakLims.SharedTestHelpers.Fakes.Sample;
using PeakLims.SharedTestHelpers.Fakes.Test;
using PeakLims.SharedTestHelpers.Fakes.Accession;
using Services;
using static TestFixture;

public class ManagePatientOnAccessionCommandTests : TestBase
{
    private readonly Faker _faker;

    public ManagePatientOnAccessionCommandTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public async Task can_manage_patient()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var patient = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(patient);
        var accession = Accession.Create();
        await testingServiceScope.InsertAsync(accession);

        // Act - set
        var command = new SetAccessionPatient.Command(accession.Id, patient.Id);
        await testingServiceScope.SendAsync(command);
        var accessionFromDb = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(x => x.Id == accession.Id));

        // Assert - set
        accessionFromDb.Patient.Id.Should().Be(patient.Id);

        // Act - remove
        var removeCommand = new RemoveAccessionPatient.Command(accession.Id);
        await testingServiceScope.SendAsync(removeCommand);
        accessionFromDb = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(x => x.Id == accession.Id));

        // Assert - remove
        accessionFromDb.Patient.Should().BeNull();
    }
}