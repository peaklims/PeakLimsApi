namespace PeakLims.Domain.Gaia.Features.Generators;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;
using PeakOrganizations;
using PeakOrganizations.Models;
using Serilog;

public record OrganizationResponse
{
    [JsonPropertyName("organizations")]
    public List<OrganizationRecord> Organizations { get; set; } = new();

    public record OrganizationRecord
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("domain")]
        public string Domain { get; set; } = string.Empty;
    }
}

public class OrganizationGenerator(IChatClient chatClient) : SimpleGeneratorBase<OrganizationResponse>(chatClient)
{
    private readonly IChatClient _chatClient = chatClient;

    
    public override async IAsyncEnumerable<OrganizationResponse> GenerateCoreAsync()
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
            Can you provide a list of 15 fake hospital or laboratory names? Here are a few examples (do not use any of these examples in your list): 

            - Greater Peach Hospital
            - Willow Creek Medical Center
            - Prairie Valley Hospital District
            - Riverbend Community Hospital
            - Sunrise Health Alliance
            - GenoQuantum Diagnostics
            - Genesight Medical
            - Redwood Labs
            
            You should also make valid email domains for each organization. For example, Greater Peach Hospital might have the domain `greaterpeachhospital.com` or `gph.com`.
            
            Make sure you return the response in valid json in the exact format below:
            
            {{jsonFormat}}
            """;
        var chatCompletion = await _chatClient.CompleteAsync(prompt, chatOptions);
        var parsedJson = JsonSerializer.Deserialize<OrganizationResponse>(chatCompletion.Message.Text);
        if (parsedJson is null)
        {
            Log.Error("Failed to parse chat completion response: {ChatCompletion}", chatCompletion);
            yield break;
        }
        
        yield return parsedJson;
    }
}

public abstract class SimpleGeneratorBase<T>(IChatClient chatClient)
{
    public abstract IAsyncEnumerable<T> GenerateCoreAsync();
}