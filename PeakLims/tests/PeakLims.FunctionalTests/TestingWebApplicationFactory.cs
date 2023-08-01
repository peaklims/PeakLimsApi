namespace PeakLims.FunctionalTests;

using PeakLims.Resources;
using PeakLims.SharedTestHelpers.Utilities;
using WebMotions.Fake.Authentication.JwtBearer;
using Configurations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Microsoft.Extensions.Logging;
using Xunit;

[CollectionDefinition(nameof(TestBase))]
public class TestingWebApplicationFactoryCollection : ICollectionFixture<TestingWebApplicationFactory> { }

public class TestingWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    
    private PostgreSqlContainer _dbContainer;
    private RabbitMqContainer _rmqContainer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Consts.Testing.FunctionalTestingEnvName);
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
        
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var functionalConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
            functionalConfig.GetSection("JaegerHost").Value = "localhost";
            configurationBuilder.AddConfiguration(functionalConfig);
        });

        builder.ConfigureServices(services =>
        {
            // add authentication using a fake jwt bearer
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
            }).AddFakeJwtBearer();

        });
    }

    public async Task InitializeAsync()
    {
        _dbContainer = new PostgreSqlBuilder().Build();
        await _dbContainer.StartAsync();
        Environment.SetEnvironmentVariable($"{ConnectionStringOptions.SectionName}__{ConnectionStringOptions.PeakLimsKey}", _dbContainer.GetConnectionString());
        // migrations applied in MigrationHostedService

        var freePort = DockerUtilities.GetFreePort();
        _rmqContainer = new RabbitMqBuilder()
            .WithPortBinding(freePort, 5672)
            .Build();
        await _rmqContainer.StartAsync();
        Environment.SetEnvironmentVariable($"{RabbitMqOptions.SectionName}__{RabbitMqOptions.HostKey}", "localhost");
        Environment.SetEnvironmentVariable($"{RabbitMqOptions.SectionName}__{RabbitMqOptions.VirtualHostKey}", "/");
        Environment.SetEnvironmentVariable($"{RabbitMqOptions.SectionName}__{RabbitMqOptions.UsernameKey}", "guest");
        Environment.SetEnvironmentVariable($"{RabbitMqOptions.SectionName}__{RabbitMqOptions.PasswordKey}", "guest");
        Environment.SetEnvironmentVariable($"{RabbitMqOptions.SectionName}__{RabbitMqOptions.PortKey}", _rmqContainer.GetConnectionString());
    }

    public new async Task DisposeAsync() 
    {
        await _dbContainer.DisposeAsync();
        await _rmqContainer.DisposeAsync();
    }
}