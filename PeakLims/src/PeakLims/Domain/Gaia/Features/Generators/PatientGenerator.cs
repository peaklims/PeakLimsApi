namespace PeakLims.Domain.Gaia.Features.Generators;

using System.Collections.Concurrent;
using Containers;
using Databases;
using Ethnicities;
using Microsoft.EntityFrameworkCore;
using Models;
using Patients;
using Npis;
using Patients.Models;
using Races;
using Samples;
using Samples.Models;
using Serilog;
using Sexes;
using Soenneker.Utils.AutoBogus;
using Utilities;

public interface IPatientGenerator
{
    Task<List<Patient>> Generate(Guid organizationId);
}

public class PatientGenerator(PeakLimsDbContext dbContext) : IPatientGenerator
{
    public async Task<List<Patient>> Generate(Guid organizationId)
    {
        var existingPatients = await dbContext.Patients
            .Include(x => x.Samples)
            .ThenInclude(x => x.Container)
            .Where(x => x.OrganizationId == organizationId)
            .ToListAsync();
        
        if (existingPatients.Count > 0)
        {
            Log.Information("Patients already exist for organization {OrganizationId} -- skipping generation", organizationId);
            return existingPatients;
        }
        
        var containerList = await dbContext.Containers
            .Include(x => x.Samples)
            .Where(x => x.OrganizationId == organizationId)
            .ToListAsync();
        return await GenerateCore(organizationId, containerList);
    }
    
    public async Task<List<Patient>> GenerateCore(Guid organizationId, List<Container> containerList)
    {
        Log.Information("Starting Patient creation");
        var random = new Random();
        // var patientCount = random.Next(135, 210);
        var patientCount = random.Next(1, 2);
        // var patientCount = random.Next(160, 181);
        var people = PersonInfoGenerator.Generate(patientCount);
        var patients = new ConcurrentBag<Patient>();
        ValueTask GeneratePatients(PersonInfo person, CancellationToken ct)
        {
            var patient = PeakLimsUtils.RunWithRetries(() => CreatePatient(person, organizationId, containerList));
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

    private static Patient CreatePatient(PersonInfo personInfo, Guid organizationId, List<Container> containerList)
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
        var patient = Patient.Create(patientToCreate);
        
        var sampleCount = Math.Min(faker.Random.Int(1, 2), containerList.Count);
        if (sampleCount == 0)
        {
            throw new InvalidOperationException("No containers available to pick for samples.");
        }
        var containersToUse = faker.PickRandom(containerList, sampleCount);
        foreach (var container in containersToUse)
        {
            var patientDob = DateOnly.Parse(patient.Lifespan.DateOfBirth.ToString());
            var sample = Sample.Create(new SampleForCreation()
            {
                Type = container.UsedFor.Value,
                CollectionDate = faker.Date.BetweenDateOnly(patientDob, DateOnly.FromDateTime(DateTime.UtcNow)),
                ExternalId = faker.Random.AlphaNumeric(faker.Random.Int(10, 20)).ToUpper(),
                ReceivedDate = faker.Date.BetweenDateOnly(patientDob, DateOnly.FromDateTime(DateTime.UtcNow)),
            });
            sample.SetContainer(container);
            patient.AddSample(sample);
        }
        
        // TODO audit is failing
        return patient;
    }
}