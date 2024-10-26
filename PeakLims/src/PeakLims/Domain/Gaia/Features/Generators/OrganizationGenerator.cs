namespace PeakLims.Domain.Gaia.Features.Generators;

using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;
using Models;
using PeakOrganizations;
using PeakOrganizations.Models;
using Resources;
using Serilog;

public interface IOrganizationGenerator
{
    Task<PeakOrganization> Generate();
}

public class OrganizationGenerator(IChatClient chatClient) : IOrganizationGenerator
{
    public async Task<PeakOrganization> Generate()
    {
        var organizationData = await GenerateData();
        Log.Information("Organization: {@Organization}", organizationData);
        var org = PeakOrganization.Create(new PeakOrganizationForCreation()
        {
            Name = organizationData.Name,
            Domain = organizationData.Domain
        });

        // TODO save
        
        return org;
    }
    
    private async Task<OrganizationResponse.OrganizationRecord> GenerateData()
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
            Can you provide a list of 5 fake laboratory names? Here are a few examples (do not use any of these examples in your list): 
            
            - Redwood Genomics
            - Greater Peach Labs
            - GenoQuantum Diagnostics
            - Genesight Medical
            - Stonebridge Labs
            - Cardinal Diagnostics
            
            You should also make valid email domains for each organization. For example, Greater Peach Hospital might have a `greaterpeachhospital.com` or `gph.com` domain. Note that the domain does NOT have an `@` symbol.
            
            Make sure you return the response in valid json in the exact format below:
            
            {{jsonFormat}}
            """;
        var chatCompletion = await chatClient.CompleteAsync(prompt, chatOptions);
        var parsedJson = JsonSerializer.Deserialize<OrganizationResponse>(chatCompletion.Message.Text, 
            JsonSerializationOptions.LlmSerializerOptions);

        var org = parsedJson.Organizations.First();
        if (org.Domain.Contains("@")) // just in case it doesn't listen
        {
            org.Domain = org.Domain.Split('@')[0];
        }
        return org;
    }
}