namespace PeakLims.IntegrationTests;

using AutoBogus;
using AutoBogus.Conventions;

[Collection(nameof(TestFixture))]
public class TestBase : IDisposable
{
    public TestBase()
    {
        AutoFaker.Configure(builder =>
        {
            // configure global autobogus settings here
            builder.WithDateTimeKind(DateTimeKind.Utc)
                .WithConventions(config =>
                {
                    // config.Email.Aliases("AnotherEmail");  // Generates an email value for members named AnotherEmail
                })
                .WithRecursiveDepth(3)
                .WithTreeDepth(1)
                .WithRepeatCount(1);
        });
    }
    
    public void Dispose()
    {
    }
}