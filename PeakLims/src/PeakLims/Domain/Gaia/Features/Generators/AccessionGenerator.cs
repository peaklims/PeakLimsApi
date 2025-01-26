namespace PeakLims.Domain.Gaia.Features.Generators;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using AccessionComments;
using Accessions;
using Bogus;
using Databases;
using HealthcareOrganizationContacts;
using HealthcareOrganizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Models;
using Panels;
using Patients;
using Resources;
using Serilog;
using Soenneker.Utils.AutoBogus;
using Tests;
using Users;
using Utilities;

public interface IAccessionGenerator
{
    Task<List<Accession>> Generate(Guid organizationId, CancellationToken cancellationToken = default);
}

public class AccessionGenerator(IChatClient chatClient, PeakLimsDbContext dbContext) : IAccessionGenerator
{
    private static readonly Faker Faker = new AutoFaker().Faker;
    
    public async Task<List<Accession>> Generate(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var existingAccessions = await dbContext.Accessions
            .Where(x => x.OrganizationId == organizationId)
            .ToListAsync(cancellationToken: cancellationToken);
        
        if (existingAccessions.Count > 0)
        {
            Log.Information("Accessions already exist for organization {OrganizationId} -- skipping generation", organizationId);
            return existingAccessions;
        }
        
        var patients = await dbContext.Patients.Where(x => x.OrganizationId == organizationId)
            .Include(x => x.Samples)
            .ThenInclude(x => x.Container)
            .ToListAsync(cancellationToken: cancellationToken);
        var healthcareOrganizations = await dbContext.HealthcareOrganizations
            .Include(x => x.HealthcareOrganizationContacts)
            .ToListAsync(cancellationToken: cancellationToken);
        var users = await dbContext.Users.ToListAsync(cancellationToken: cancellationToken);

        var panelsAndTests = new PanelTestResponse
        {
            Panels = await dbContext.Panels.Where(x => x.OrganizationId == organizationId).ToListAsync(cancellationToken: cancellationToken),
            StandaloneTests = await dbContext.Tests.Where(x => x.OrganizationId == organizationId).ToListAsync(cancellationToken: cancellationToken)
        };
        
        var accessions = await GenerateCore(patients, healthcareOrganizations, panelsAndTests, users);
        await dbContext.Accessions.AddRangeAsync(accessions, cancellationToken);
        
        return accessions;
    }
    
    public async Task<List<Accession>> GenerateCore(List<Patient> patients, List<HealthcareOrganization> healthcareOrganizations, 
        PanelTestResponse panelsAndTests, List<User> users)
    {
        Log.Information("Starting Accession creation");
        
        var accessions = new ConcurrentBag<Accession>();
        var patientBag = new ConcurrentBag<Patient>(patients);
        var orgBag = new ConcurrentBag<HealthcareOrganization>(healthcareOrganizations);
        var userBag = new ConcurrentBag<User>(users);
        var panelBag = new ConcurrentBag<Panel>(panelsAndTests.Panels);
        var testBag = new ConcurrentBag<Test>(panelsAndTests.StandaloneTests);

        async ValueTask GenerateAccessions(Patient patient, CancellationToken ct)
        {
            var healthOrg = Faker.PickRandom(orgBag.ToList());
            var accession = await PeakLimsUtils.RunWithRetriesAsync(() => CreateAccession(patient, healthOrg, 
                panelBag, userBag));
            accessions.Add(accession);
        }
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 100
        };
        await Parallel.ForEachAsync(patientBag, options, GenerateAccessions);

        Log.Information("Accessions created: {AccessionCount}", patients.Count);
        return [.. accessions];
    }

    private async Task<Accession> CreateAccession(Patient patient, HealthcareOrganization healthOrg,
        ConcurrentBag<Panel> panels, ConcurrentBag<User> users)
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

        var startedAt = Stopwatch.GetTimestamp();
        var conversation = await GenerateConversation(accession, users);
        var delta = Stopwatch.GetElapsedTime(startedAt);
        Log.Information("Conversation generated in {Delta} for accession {AccessionNumber}", delta, accession.AccessionNumber);
        
        foreach (var comment in conversation.Conversation)
        {
            if (comment.OriginalCommentText != null)
            {
                var accessionComment = AccessionComment.Create(accession, comment.OriginalCommentText, comment.UserIdentifier);
                accessionComment.Update(comment.CommentText, comment.UserIdentifier, out var newComment, out var archivedComment);
                dbContext.AccessionComments.Add(archivedComment);
                dbContext.AccessionComments.Add(newComment);
                continue;
            }
            
            var soloAccessionComment = AccessionComment.Create(accession, comment.CommentText, comment.UserIdentifier);
            dbContext.AccessionComments.Add(soloAccessionComment);
        }
        
        return accession;
    }
    
     private async Task<CommentConversationResponse> GenerateConversation(Accession accession, ConcurrentBag<User> users)
     {
         var chatOptions = new ChatOptions
         {
             ResponseFormat = ChatResponseFormat.Json,
         };
         var jsonFormat = 
             // lang=json
             """
             {
                 "conversation": [
                     { "commentText": "string", "originalCommentText": "string or null", "userIdentifier": "string", "orderInConversation": "int" }
                 ]
             }
             """;
         
         var userIdentifiersForPrompt = string.Join(", ", users.Select(x => $"`{x.Identifier}`"));
         var orgContactsForPrompt = string.Join(", ", accession.HealthcareOrganization
             .HealthcareOrganizationContacts.Select(x => $"{x.FirstName} {x.LastName}"));
         var testInfoForPrompt = string.Join(", ", accession.TestOrders.Select(x => $"A test called '{x.Test.TestName}' with a '{x.Test.Methodology}' methodology and a '{x.Test.TurnAroundTime}' turnaround time"));
         var sampleInfoForPrompt = string.Join(", ", accession.Patient.Samples.Select(x => $"{x.Type.Value} with a sample number of {x.SampleNumber} in a {x.Container}"));
         
         var prompt =
             $$"""
               Your objective is to help me generate demo data for an accession in a LIMS. Specifically, I need to 
               create a conversation between a 2 or more users about a given accession.
               
               This conversation should be realistic in the context of a conversation that may actually happen in a 
               laboratory setting. For example, it could be about the handling of the sample, the prep work for the test,
               contacting someone at the ordering org for more information, any issues that may have arisen, etc.
               
               The conversation could be just a single comment from a user, but it could also be a back-and-forth between
               multiple users. 
               
               The user identifiers that can be involved in this conversation are: {{userIdentifiersForPrompt}} 
               No other user identifiers should be used in the conversations.
               
               The ordering organization name is: {{accession.HealthcareOrganization.Name}}
               The contacts at the ordering organization are: {{orgContactsForPrompt}}
               The samples on the accession are: {{sampleInfoForPrompt}}
               The tests that have been ordered are {{testInfoForPrompt}}
               
               Please use this information to create a conversation that is realistic and relevant to the context of the 
               conversation. Don't make up entities or intentifiers that are not part of this context.

               Depending on the context of the conversation, you can add an edit history for a comment to show that a 
               user may have corrected a typo or expanded on a thought.

               Make sure you return the response in valid json in the exact format below:

               {{jsonFormat}}
               
               The `commentText` should be the text of the comment.
               The `originalCommentText` should be the text of the comment before any edits. If there are no edits, this would be null.
               The `userIdentifier` should be the identifier of the user who made the comment.
               The `orderInConversation` should be the order in which the comment was made in the conversation.
               
               Here is an example of a conversation with dummy data:
               ```
               {
                   "conversation": [
                       { 
                           "commentText": "Please note that sample number 12345 arrived hemolyzed. We need to contact the ordering organization for a redraw.",
                           "originalCommentText": "Please note that sample number 12345 arrived hemolyzed. We need to contact the ordering organization for a redaw.",
                           "userIdentifier": "jdoe",
                           "orderInConversation": 1
                       },
                       {
                           "commentText": "Understood. I'll reach out to Dr. Emily Watson to arrange a new sample collection.",
                           "originalCommentText": null,
                           "userIdentifier": "asmith",
                           "orderInConversation": 2
                       },
                       {
                           "commentText": "Update: Dr. Watson has scheduled a redraw for tomorrow morning.",
                           "originalCommentText": null,
                           "userIdentifier": "asmith",
                           "orderInConversation": 3
                       },
                       {
                           "commentText": "Great, thanks for handling that.",
                           "originalCommentText": null,
                           "userIdentifier": "bjones",
                           "orderInConversation": 4
                       }
                   ]
               }
               ```
               """;
         var chatCompletion = await chatClient.CompleteAsync(prompt, chatOptions);
         var conversation = JsonSerializer.Deserialize<CommentConversationResponse>(chatCompletion.Message.Text, 
             JsonSerializationOptions.LlmSerializerOptions);
         
        conversation.Conversation = [.. conversation.Conversation.OrderBy(x => x.OrderInConversation)];
        return conversation;
     }
}