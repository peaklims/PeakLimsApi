namespace PeakLims.IntegrationTests;

// No [Collection] attribute: each test class is its own collection so classes run in
// parallel against each other, all sharing the single assembly-level TestFixture
// (see AssemblyFixtureRegistration.cs). AutoFaker is configured once in the fixture.
public class TestBase : IDisposable
{
    public void Dispose()
    {
    }
}
