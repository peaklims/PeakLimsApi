namespace PeakLims.Domain.Gaia.Features.Generators;

using System.Collections.Concurrent;
using Accessions;
using Bogus;
using HealthcareOrganizationContacts;
using HealthcareOrganizations;
using Patients;
using Serilog;
using Soenneker.Utils.AutoBogus;

public interface IAccessionGenerator
{
    Task<List<Accession>> Generate(List<Patient> patients, List<HealthcareOrganization> healthcareOrganizations);
}

public class AccessionGenerator : IAccessionGenerator
{
    private static readonly Faker Faker = new AutoFaker().Faker;
    
    public async Task<List<Accession>> Generate(List<Patient> patients, List<HealthcareOrganization> healthcareOrganizations)
    {
        Log.Information("Starting Accession creation");
        var accessions = new ConcurrentBag<Accession>();
        var patientBag = new ConcurrentBag<Patient>(patients);
        var orgBag = new ConcurrentBag<HealthcareOrganization>(healthcareOrganizations);
        ValueTask GenerateAccessions(Patient patient, CancellationToken ct)
        {
            var healthOrg = Faker.PickRandom(orgBag.ToList());
            var accession = CreateAccession(patient, healthOrg);
            accessions.Add(accession);
            return ValueTask.CompletedTask;
        }
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 100
        };
        await Parallel.ForEachAsync(patientBag, options, GenerateAccessions);

        Log.Information("Accessions created: {AccessionCount}", patients.Count);
        return accessions.ToList();
    }

    private static Accession CreateAccession(Patient patient, HealthcareOrganization healthOrg)
    {
        var random = new Random();
        var contactCount = random.Next(2, 6);
        var contacts = new ConcurrentBag<HealthcareOrganizationContact>();
        for (var i = 0; i < contactCount; i++)
        {
            var contact = Faker.PickRandom(healthOrg.HealthcareOrganizationContacts);
            contacts.Add(contact);
        }
        
        var accession = Accession.Create(patient.OrganizationId);
        accession.SetPatient(patient)
            .SetHealthcareOrganization(healthOrg);
        
        foreach (var healthcareOrganizationContact in contacts)
        {
            accession.AddContact(healthcareOrganizationContact);
        }
        
        return accession;
    }
}