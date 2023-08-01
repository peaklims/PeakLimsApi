namespace KeycloakInPulumi;

using System.Threading.Tasks;
using Pulumi;

internal static class Program
{
    private static Task<int> Main() => Deployment.RunAsync<RealmBuild>();
}