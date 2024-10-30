namespace PeakLims.Domain.Gaia.Features.Generators;

using System.Collections.Concurrent;
using System.Text.Json;
using AccessionComments;
using Accessions;
using Bogus;
using HealthcareOrganizationContacts;
using HealthcareOrganizations;
using Microsoft.Extensions.AI;
using Models;
using Panels;
using Patients;
using Resources;
using Serilog;
using Soenneker.Utils.AutoBogus;
using Tests;
using Users;

public interface IAccessionGenerator
{
    Task<List<Accession>> Generate(List<Patient> patients, List<HealthcareOrganization> healthcareOrganizations,
        PanelTestResponse panelsAndTests, List<User> users);
}

public class AccessionGenerator(IChatClient chatClient) : IAccessionGenerator
{
    private static readonly Faker Faker = new AutoFaker().Faker;
    
    public async Task<List<Accession>> Generate(List<Patient> patients, List<HealthcareOrganization> healthcareOrganizations, 
        PanelTestResponse panelsAndTests, List<User> users)
    {
        Log.Information("Starting Accession creation");
        var accessions = new ConcurrentBag<Accession>();
        var patientBag = new ConcurrentBag<Patient>(patients);
        var orgBag = new ConcurrentBag<HealthcareOrganization>(healthcareOrganizations);
        var userBag = new ConcurrentBag<User>(users);
        var panelBag = new ConcurrentBag<Panel>(panelsAndTests.Panels);
        var testBag = new ConcurrentBag<Test>(panelsAndTests.StandaloneTests);
        ValueTask GenerateAccessions(Patient patient, CancellationToken ct)
        {
            var healthOrg = Faker.PickRandom(orgBag.ToList());
            var accession = CreateAccession(patient, healthOrg, panelBag, testBag, userBag);
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

    private static Accession CreateAccession(Patient patient, HealthcareOrganization healthOrg,
        ConcurrentBag<Panel> panels, ConcurrentBag<Test> tests, ConcurrentBag<User> users)
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
        
        var panel = Faker.PickRandom(panels.ToList());
        accession.AddPanel(panel);
        
        foreach (var testOrder in accession.TestOrders)
        {
            if (testOrder.Test.TestName.Contains("Proband")) // if it doesn't i need to use relatives which i haven't made yet
            {
                try
                {
                    var sample = Faker.PickRandom(patient.Samples);
                    testOrder.SetSample(sample);
                }
                catch (Exception e)
                {
                    // this is okay and expected on occasion if the sample type isn't valid for the test. don't mind
                    // those tests not getting samples for seed
                    Log.Error("Could not set sample for test order: {Error}", e.Message);
                }
            }
        }
        
        return accession;
    }
    
    
//     private async Task<List<AccessionComment>> GenerateConversation(Accession accession)
//     {
//         var chatOptions = new ChatOptions
//         {
//             ResponseFormat = ChatResponseFormat.Json,
//         };
//         var jsonFormat = 
//             // lang=json
//             """
//             {
//                 "organizations": [
//                     { "name": "string", "domain": "string" }
//                 ]
//             }
//             """;
//         var prompt =
//             $$"""
//               Can you provide a list of 5 fake laboratory names? Here are a few examples (do not use any of these examples in your list): 
//
//               - Redwood Genomics
//               - Greater Peach Labs
//               - GenoQuantum Diagnostics
//               - Genesight Medical
//               - Stonebridge Labs
//               - Cardinal Diagnostics
//
//               You should also make valid email domains for each organization. For example, Greater Peach Hospital might have a `greaterpeachhospital.com` or `gph.com` domain. Note that the domain does NOT have an `@` symbol.
//
//               Make sure you return the response in valid json in the exact format below:
//
//               {{jsonFormat}}
//               """;
//         var chatCompletion = await chatClient.CompleteAsync(prompt, chatOptions);
//         var parsedJson = JsonSerializer.Deserialize<OrganizationResponse>(chatCompletion.Message.Text, 
//             JsonSerializationOptions.LlmSerializerOptions);
//
//         var org = parsedJson.Organizations.First();
//         if (org.Domain.Contains("@")) // just in case it doesn't listen
//         {
//             org.Domain = org.Domain.Split('@')[0];
//         }
//         return org;
//     }
}