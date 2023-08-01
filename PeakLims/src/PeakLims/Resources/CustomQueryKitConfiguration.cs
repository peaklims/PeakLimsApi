namespace PeakLims.Resources;

using QueryKit.Configuration;

public class CustomQueryKitConfiguration : QueryKitConfiguration
{
    public CustomQueryKitConfiguration(Action<QueryKitSettings>? configureSettings = null)
        : base(settings => 
        {
            // configure custom global settings here
            // settings.EqualsOperator = "eq";

            configureSettings?.Invoke(settings);
        })
    {
    }
}