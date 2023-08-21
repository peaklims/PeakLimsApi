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
    public async Task can_manage_existing_patients()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var patient = new FakePatientBuilder().Build();
        await testingServiceScope.InsertAsync(patient);
        var accession = Accession.Create();
        await testingServiceScope.InsertAsync(accession);

        // Act - set
        var command = new SetAccessionPatient.Command(accession.Id, patient.Id, null);
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
    
    [Fact]
    public async Task can_add_a_patient()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var accession = Accession.Create();
        await testingServiceScope.InsertAsync(accession);
        var patientToAdd = new FakePatientForCreationDto().Generate();

        // Act
        var command = new SetAccessionPatient.Command(accession.Id, null, patientToAdd);
        await testingServiceScope.SendAsync(command);
        var accessionFromDb = await testingServiceScope.ExecuteDbContextAsync(db => db.Accessions
            .FirstOrDefaultAsync(x => x.Id == accession.Id));

        // Assert
        accessionFromDb.Patient.FirstName.Should().Be(patientToAdd.FirstName);
        accessionFromDb.Patient.LastName.Should().Be(patientToAdd.LastName);
        accessionFromDb.Patient.Lifespan.DateOfBirth.Should().Be(patientToAdd.DateOfBirth);
        accessionFromDb.Patient.Race.Value.Should().Be(patientToAdd.Race);
        accessionFromDb.Patient.Ethnicity.Value.Should().Be(patientToAdd.Ethnicity);
    }
}