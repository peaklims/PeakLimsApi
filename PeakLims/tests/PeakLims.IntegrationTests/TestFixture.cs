namespace PeakLims.IntegrationTests;

using System.Security.Claims;
using Amazon.S3;
using PeakLims.Extensions.Services;
using PeakLims.Databases;
using PeakLims.Resources;
using PeakLims.SharedTestHelpers.Utilities;
using FluentAssertions;
using FluentAssertions.Extensions;
using Hangfire;
using HeimGuard;
using NSubstitute;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharedTestHelpers.Fakes.PeakOrganization;
using Testcontainers.LocalStack;
using static Resources.PeakLimsOptions;

[CollectionDefinition(nameof(TestFixture))]
public class TestFixtureCollection : ICollectionFixture<TestFixture> {}

public class TestFixture : IAsyncLifetime
{
    public static IServiceScopeFactory BaseScopeFactory;
    private PostgreSqlContainer _dbContainer;
    private RabbitMqContainer _rmqContainer;
    private LocalStackContainer _localStackContainer;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Consts.Testing.IntegrationTestingEnvName
        });

        _dbContainer = new PostgreSqlBuilder().Build();
        await _dbContainer.StartAsync();
        builder.Configuration.GetSection(ConnectionStringOptions.SectionName)[ConnectionStringOptions.PeakLimsKey] = _dbContainer.GetConnectionString();
        await RunMigration(_dbContainer.GetConnectionString());

        var freePort = DockerUtilities.GetFreePort();
        _rmqContainer = new RabbitMqBuilder()
            .WithPortBinding(freePort, 5672)
            .Build();
        await _rmqContainer.StartAsync();
        builder.Configuration.GetSection(RabbitMqOptions.SectionName)[RabbitMqOptions.HostKey] = "localhost";
        builder.Configuration.GetSection(RabbitMqOptions.SectionName)[RabbitMqOptions.VirtualHostKey] = "/";
        builder.Configuration.GetSection(RabbitMqOptions.SectionName)[RabbitMqOptions.UsernameKey] = "guest";
        builder.Configuration.GetSection(RabbitMqOptions.SectionName)[RabbitMqOptions.PasswordKey] = "guest";
        builder.Configuration.GetSection(RabbitMqOptions.SectionName)[RabbitMqOptions.PortKey] = _rmqContainer.GetConnectionString();

        var localstackPort = DockerUtilities.GetFreePort();
        builder.Configuration["PeakLims:LocalstackPort"] = localstackPort.ToString();
        _localStackContainer = new LocalStackBuilder()
            .WithPortBinding(localstackPort, 4566)
            .Build();
        await _localStackContainer.StartAsync();
        await AddS3Buckets(localstackPort);
        
        builder.ConfigureServices();
        var services = builder.Services;

        // add any mock services here
        services.ReplaceServiceWithSingletonMock<IHttpContextAccessor>();
        services.ReplaceServiceWithSingletonMock<IHeimGuardClient>();
        services.ReplaceServiceWithSingletonMock<IBackgroundJobClient>();

        var provider = services.BuildServiceProvider();
        BaseScopeFactory = provider.GetService<IServiceScopeFactory>();
        
        var dbContext = provider.GetService<PeakLimsDbContext>();
        var testingOrganization = new FakePeakOrganizationBuilder().Build();
        testingOrganization.OverrideId(TestingConsts.DefaultTestingOrganizationId);
        await dbContext.PeakOrganizations.AddAsync(testingOrganization);
        await dbContext.SaveChangesAsync();
        
        SetupDateAssertions();
    }
    
    private static async Task RunMigration(string connectionString)
    {
        var options = new DbContextOptionsBuilder<PeakLimsDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;
        var context = new PeakLimsDbContext(options, null, null, null);
        await context?.Database?.MigrateAsync();
    }

    private static async Task AddS3Buckets(int localstackPort)
    {
        var config = new AmazonS3Config { ForcePathStyle = true, ServiceURL = $"http://localhost:{localstackPort}" };
        var client = new AmazonS3Client("test", "test", config);
        foreach (var bucket in Consts.S3Buckets.List())
        {
            await client.PutBucketAsync(bucket);
        }
    }

    public async Task DisposeAsync()
    {        
        await _dbContainer.DisposeAsync();
        await _rmqContainer.DisposeAsync();
        await _localStackContainer.DisposeAsync();
    }

    private static void SetupDateAssertions()
    {
        // close to equivalency required to reconcile precision differences between EF and Postgres
        AssertionOptions.AssertEquivalencyUsing(options =>
        {
            options.Using<DateTime>(ctx => ctx.Subject
                .Should()
                .BeCloseTo(ctx.Expectation, 1.Seconds())).WhenTypeIs<DateTime>();
            options.Using<DateTimeOffset>(ctx => ctx.Subject
                .Should()
                .BeCloseTo(ctx.Expectation, 1.Seconds())).WhenTypeIs<DateTimeOffset>();

            return options;
        });
    }
}

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection ReplaceServiceWithSingletonMock<TService>(this IServiceCollection services)
        where TService : class
    {
        services.RemoveAll(typeof(TService));
        services.AddSingleton(_ => Substitute.For<TService>());
        return services;
    }
}
