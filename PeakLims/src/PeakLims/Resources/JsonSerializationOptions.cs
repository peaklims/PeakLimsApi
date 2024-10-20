namespace PeakLims.Resources;

using System.Text.Json;

public static class JsonSerializationOptions
{
    public readonly static JsonSerializerOptions LlmSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
}