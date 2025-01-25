namespace PeakLims.Utilities;

using System.Reflection;
using System.Text.Json;

public static class JsonHelpers
{ 
    public static object? DeserializeWithReflection(Type dataType, string json)
    {
        var method = typeof(JsonSerializer)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == nameof(JsonSerializer.Deserialize) && m.IsGenericMethod)
            .Where(m =>
            {
                var parameters = m.GetParameters();
                return parameters.Length == 2
                       && parameters[0].ParameterType == typeof(string)
                       && parameters[1].ParameterType == typeof(JsonSerializerOptions);
            })
            .Single(); // There's only one matching overload with these two parameters

        // 2. Make the generic method using 'dataType'
        var genericMethod = method.MakeGenericMethod(dataType);

        // 3. Invoke the method. Pass `null` for the JsonSerializerOptions if desired.
        return genericMethod.Invoke(null, [json, null]);
    }
}