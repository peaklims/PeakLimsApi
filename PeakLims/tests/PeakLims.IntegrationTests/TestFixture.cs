namespace PeakLims.IntegrationTests;

using System.Security.Claims;
using Amazon.S3;
using DotNet.Testcontainers.Builders;
using AutoBogus;
using AutoBogus.Conventions;
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

public class TestFixture : IAsyncLifetime
{
    public static IServiceScopeFactory BaseScopeFactory;
    private PostgreSqlContainer _dbContainer;
    private RabbitMqContainer _rmqContainer;
    private LocalStackContainer _localStackContainer;

    // Reuse the containers across local test runs so the container-startup floor (which
    // dominates the fixture's wall-clock time) is paid once instead of on every run.
    // Disabled in CI: ephemeral runners can't reconnect to a prior container, and a fresh
    // database per job is safer there.
    private static readonly bool ReuseContainers =
        Environment.GetEnvironmentVariable("CI") is null;

    public async ValueTask InitializeAsync()
    {
        ConfigureAutoFaker();
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Consts.Testing.IntegrationTestingEnvName
        });

        _dbContainer = new PostgreSqlBuilder()
            .WithReuse(ReuseContainers)
            .WithLabel("peaklims.reuse", "integration-db")
            .Build();

        _rmqContainer = new RabbitMqBuilder()
            .WithReuse(ReuseContainers)
            .WithLabel("peaklims.reuse", "integration-rmq")
            // Same reuse concern as LocalStack: the default RabbitMQ wait strategy is
            // log-based and hangs on a reused container. A port probe is re-runnable and
            // sufficient here (nothing in the suite actually connects to the broker).
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
            .Build();

        _localStackContainer = new LocalStackBuilder()
            .WithReuse(ReuseContainers)
            .WithLabel("peaklims.reuse", "integration-localstack")
            // Boot only S3 (the sole AWS service the suite exercises) rather than the full
            // LocalStack service set - this cuts container readiness time substantially.
            .WithEnvironment("SERVICES", "s3")
            .WithEnvironment("EAGER_SERVICE_LOADING", "1")
            // Poll the health endpoint instead of the default log-message wait. A log-based
            // wait hangs on a reused container because the "Ready." line was already emitted
            // on the original boot and never reappears; an HTTP probe succeeds either way.
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request
                    .ForPort(4566)
                    .ForPath("/_localstack/health")))
            .Build();

        // Container startup dominates the fixture's wall-clock time, so start all three
        // containers concurrently instead of awaiting each one in turn.
        await Task.WhenAll(
            _dbContainer.StartAsync(),
            _rmqContainer.StartAsync(),
            _localStackContainer.StartAsync());

        // Let Docker assign ephemeral host ports and read them back after start. Fixed
        // GetFreePort() bindings both broke reuse (the host port feeds the reuse hash, so it
        // changed every run) and raced other processes for the port ("address already in use").
        var localstackPort = _localStackContainer.GetMappedPublicPort(4566);

        builder.Configuration.GetSection(ConnectionStringOptions.SectionName)[ConnectionStringOptions.PeakLimsKey] = _dbContainer.GetConnectionString();
        builder.Configuration.GetSection(RabbitMqOptions.SectionName)[RabbitMqOptions.HostKey] = "localhost";
        builder.Configuration.GetSection(RabbitMqOptions.SectionName)[RabbitMqOptions.VirtualHostKey] = "/";
        builder.Configuration.GetSection(RabbitMqOptions.SectionName)[RabbitMqOptions.UsernameKey] = "guest";
        builder.Configuration.GetSection(RabbitMqOptions.SectionName)[RabbitMqOptions.PasswordKey] = "guest";
        builder.Configuration.GetSection(RabbitMqOptions.SectionName)[RabbitMqOptions.PortKey] = _rmqContainer.GetConnectionString();
        builder.Configuration["PeakLims:LocalstackPort"] = localstackPort.ToString();

        // Post-start setup that each depends on a running container - also run concurrently.
        await Task.WhenAll(
            RunMigration(_dbContainer.GetConnectionString()),
            AddS3Buckets(localstackPort));

        builder.ConfigureServices();
        var services = builder.Services;

        // Add any mock services here.
        // These are registered as *scoped* so each TestingServiceScope gets its own instance.
        // This isolates per-test state (permissions, enqueued jobs) so tests can run in parallel.
        // IHttpContextAccessor is intentionally left as the real implementation: its internal
        // AsyncLocal isolates the "current user" per async flow, which is exactly what parallel
        // tests need (see TestingServiceScope.SetUser).
        services.ReplaceServiceWithScopedMock<IHeimGuardClient>();
        services.ReplaceServiceWithScopedMock<IBackgroundJobClient>();

        var provider = services.BuildServiceProvider();
        BaseScopeFactory = provider.GetService<IServiceScopeFactory>();
        
        var dbContext = provider.GetService<PeakLimsDbContext>();
        await SeedDefaultTestingOrganization(dbContext);

        SetupDateAssertions();
    }

    private static async Task SeedDefaultTestingOrganization(PeakLimsDbContext dbContext)
    {
        // Idempotent: a reused DB container already holds this row from a prior run, and
        // re-inserting the fixed id would violate the primary key.
        var alreadySeeded = await dbContext.PeakOrganizations
            .AnyAsync(o => o.Id == TestingConsts.DefaultTestingOrganizationId);
        if (alreadySeeded)
            return;

        var testingOrganization = new FakePeakOrganizationBuilder().Build();
        testingOrganization.OverrideId(TestingConsts.DefaultTestingOrganizationId);
        await dbContext.PeakOrganizations.AddAsync(testingOrganization);
        await dbContext.SaveChangesAsync();
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
        var existing = await client.ListBucketsAsync();
        foreach (var bucket in Consts.S3Buckets.List())
        {
            // Idempotent: a reused LocalStack container may already hold the bucket.
            var alreadyExists = existing.Buckets?.Any(b => b.BucketName == bucket) ?? false;
            if (!alreadyExists)
                await client.PutBucketAsync(bucket);
        }
    }

    public async ValueTask DisposeAsync()
    {
        // When reusing, leave the containers running so the next local run reconnects to them
        // instead of paying the startup floor again.
        if (ReuseContainers)
            return;

        await Task.WhenAll(
            _dbContainer.DisposeAsync().AsTask(),
            _rmqContainer.DisposeAsync().AsTask(),
            _localStackContainer.DisposeAsync().AsTask());
    }

    private static void ConfigureAutoFaker()
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

    public static IServiceCollection ReplaceServiceWithScopedMock<TService>(this IServiceCollection services)
        where TService : class
    {
        services.RemoveAll(typeof(TService));
        services.AddScoped(_ => Substitute.For<TService>());
        return services;
    }
}
