namespace PeakLims.Domain.Gaia.Features.Generators;

using System.Collections.Concurrent;
using Ethnicities;
using Models;
using Patients;
using Npis;
using Patients.Models;
using Races;
using Serilog;
using Sexes;
using Soenneker.Utils.AutoBogus;

public static class PatientGenerator
{
    public static async Task<List<Patient>> Generate(Guid organizationId)
    {
        Log.Information("Starting Patient creation");
        var random = new Random();
        var patientCount = random.Next(135, 210);
        var people = PersonInfoGenerator.Generate(patientCount);
        var patients = new ConcurrentBag<Patient>();
        ValueTask GeneratePatients(PersonInfo person, CancellationToken ct)
        {
            var patient = CreatePatient(person, organizationId);
            patients.Add(patient);
            return ValueTask.CompletedTask;
        }
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 100
        };
        await Parallel.ForEachAsync(people, options, GeneratePatients);

        Log.Information("Patients created: {PatientCount}", patients.Count);
        return patients.ToList();
    }

    private static Patient CreatePatient(PersonInfo personInfo, Guid organizationId)
    {
        var faker = new AutoFaker().Faker;
        var patientToCreate = new PatientForCreation()
        {
            FirstName = personInfo.FirstName,
            LastName = personInfo.LastName,
            Sex = Sex.Of(personInfo.Sex.ToString()),
            DateOfBirth = faker.Date.PastDateOnly(92),
            Race = faker.PickRandom(Race.ListNames()),
            Ethnicity = faker.PickRandom(Ethnicity.ListNames()),
            OrganizationId = organizationId
        };
        
        // TODO audit is failing
        return Patient.Create(patientToCreate);
    }
}