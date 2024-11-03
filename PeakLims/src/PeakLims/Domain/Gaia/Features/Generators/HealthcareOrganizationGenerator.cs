namespace PeakLims.Domain.Gaia.Features.Generators;

using System.Text.Json;
using System.Text.RegularExpressions;
using HealthcareOrganizations;
using HealthcareOrganizations.Models;
using Microsoft.Extensions.AI;
using Models;
using PeakOrganizations;
using PeakOrganizations.Models;
using Resources;
using Serilog;

public interface IHealthcareOrganizationGenerator
{
    Task<List<HealthcareOrganization>> Generate(PeakOrganization organization);
}

public class HealthcareOrganizationGenerator(IChatClient chatClient) : IHealthcareOrganizationGenerator
{
    public async Task<List<HealthcareOrganization>> Generate(PeakOrganization organization)
    {
        var healthcareOrgInfo = await GenerateData();
        
        Log.Information("Converting Healthcare Org Info to Healthcare Organizations");
        var healthcareOrganizations = new List<HealthcareOrganization>();
        foreach (var org in healthcareOrgInfo)
        {
            var domainOrg = HealthcareOrganization.Create(new HealthcareOrganizationForCreation()
            {
                Name = org.Name,
                KnownDomain = org.Domain,
                OrganizationId = organization.Id
            });
            healthcareOrganizations.Add(domainOrg);
        }
        
        return healthcareOrganizations;
    }
    
    private async Task<List<HealthcareOrganizationResponse.OrganizationRecord>> GenerateData()
    {
        var chatOptions = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.Json,
        };
        var jsonFormat = 
            // lang=json
            """
            {
                "organizations": [
                    { "name": "string", "domain": "string" }
                ]
            }
            """;
        var prompt =
            $$"""
            Can you provide a list of 30 fake hospital names? Here are a few examples (you can use some of these in your example list): 

            - Greater Peach Hospital
            - Willow Creek Medical Center
            - Prairie Valley Hospital District
            - Riverbend Community Hospital
            - Sunrise Health Alliance
            - Blue Ridge Medical Center
            
            Make sure there are no repeat names.
            
            You should also make valid email domains for each organization. For example, Greater Peach Hospital might have the 
            domain `greaterpeachhospital.com` or `gph.com`.
            
            Make sure you return the response in valid json in the exact format below:
            
            {{jsonFormat}}
            """;
        var chatCompletion = await chatClient.CompleteAsync(prompt, chatOptions);
        var parsedJson = JsonSerializer.Deserialize<HealthcareOrganizationResponse>(chatCompletion.Message.Text,
            JsonSerializationOptions.LlmSerializerOptions);

        Log.Information("Healthcare Org Info: {@HealthcareOrgInfo}", parsedJson.Organizations.Select(x => new { x.Name, x.Domain }));
        return parsedJson.Organizations;
    }
}