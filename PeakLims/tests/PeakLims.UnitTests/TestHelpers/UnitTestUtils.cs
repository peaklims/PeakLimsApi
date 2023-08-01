namespace PeakLims.UnitTests.TestHelpers;

using System.Reflection;
using Services;

public class UnitTestUtils
{
    public static Assembly GetApiAssembly()
    {
        // need to load something from the api for it to be in the loaded assemblies
        _ = new DateTimeProvider();
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "PeakLims");
    }
}
