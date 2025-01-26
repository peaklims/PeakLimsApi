namespace PeakLims.UnitTests.TestHelpers;

using System.Reflection;
using Resources;
using Services;

public class UnitTestUtils
{
    public static Assembly GetApiAssembly()
    {
        // Force a direct reference to a type in PeakLims:
        var _ = typeof(Program); // or typeof(SomeController)
        return typeof(Program).Assembly;
        
        // need to load something from the api for it to be in the loaded assemblies
        // _ = Consts.Testing.IntegrationTestingEnvName;
        // return AppDomain.CurrentDomain
        //     .GetAssemblies()
        //     .FirstOrDefault(a => a.GetName().Name == "PeakLims");
    }
}
