namespace PeakLims.Extensions.Services;

using Amazon.S3;
using PeakLims.Databases;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using HeimGuard;
using PeakLims.Resources;
using PeakLims.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Resources.HangfireUtilities;

public static class ServiceRegistration
{
    public static void AddInfrastructure(this IServiceCollection services, 
        IWebHostEnvironment env, 
        IConfiguration configuration)
    {
        // DbContext -- Do Not Delete
        var connectionString = configuration.GetConnectionStringOptions().PeakLims;
        if(string.IsNullOrWhiteSpace(connectionString))
        {
            // this makes local migrations easier to manage. feel free to refactor if desired.
            connectionString = env.IsDevelopment() 
                ? "Host=localhost;Port=38869;Database=dev_peaklims;Username=postgres;Password=postgres"
                : throw new Exception("The database connection string is not set.");
        }

        services.AddDbContext<PeakLimsDbContext>(options =>
            options.UseNpgsql(connectionString,
                builder => builder.MigrationsAssembly(typeof(PeakLimsDbContext).Assembly.FullName))
                            .UseSnakeCaseNamingConvention());

        services.AddHostedService<MigrationHostedService<PeakLimsDbContext>>();
        services.SetupHangfire(env);

        // Auth -- Do Not Delete
        var authOptions = configuration.GetAuthOptions();
        if (!env.IsEnvironment(Consts.Testing.FunctionalTestingEnvName))
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authOptions.Authority;
                    options.Audience = authOptions.Audience;
                    options.RequireHttpsMetadata = !env.IsDevelopment();
                });
        }

        services.AddAuthorization(options =>
        {
        });

        services.AddHeimGuard<UserPolicyHandler>()
            .MapAuthorizationPolicies()
            .AutomaticallyCheckPermissions();
        
        if(env.IsDevelopment() 
           || env.IsEnvironment(Consts.Testing.IntegrationTestingEnvName)
           || env.IsEnvironment(Consts.Testing.FunctionalTestingEnvName))
        {
            services.AddSingleton<IAmazonS3>(_ =>
            {
                var localstackPort = configuration.GetLocalstackPort();
                var config = new AmazonS3Config { ForcePathStyle = true, ServiceURL = $"http://localhost:{localstackPort}" };
                var client = new AmazonS3Client("test", "test", config);

                foreach (var bucket in Consts.S3Buckets.List())
                {
                    client.PutBucketAsync(bucket);
                }
            
                return client;
            });
        }
        else
        {
            throw new NotImplementedException("S3 is not configured for non-local environments.");
        }
    }
}
    
public static class HangfireConfig
{
    public static void SetupHangfire(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddScoped<IJobContextAccessor, JobContextAccessor>();
        services.AddScoped<IJobWithUserContext, JobWithUserContext>();
        // if you want tags with sql server
        // var tagOptions = new TagsOptions() { TagsListStyle = TagsListStyle.Dropdown };
        
        // var hangfireConfig = new MemoryStorageOptions() { };
        services.AddHangfire(config =>
        {
            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseMemoryStorage()
                .UseColouredConsoleLogProvider()
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                // if you want tags with sql server
                // .UseTagsWithSql(tagOptions, hangfireConfig)
                .UseActivator(new JobWithUserContextActivator(services.BuildServiceProvider()
                    .GetRequiredService<IServiceScopeFactory>()));
        });
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 10;
            options.ServerName = $"PeakLims-{env.EnvironmentName}";

            if (Consts.HangfireQueues.List().Length > 0)
            {
                options.Queues = Consts.HangfireQueues.List();
            }
        });

    }
}
