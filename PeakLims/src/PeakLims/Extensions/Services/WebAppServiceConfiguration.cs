namespace PeakLims.Extensions.Services;

using PeakLims.Middleware;
using PeakLims.Services;
using System.Text.Json.Serialization;
using Serilog;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Resources;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;

public static class WebAppServiceConfiguration
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<PeakLimsOptions>(builder.Configuration.GetSection(PeakLimsOptions.SectionName));

        builder.Services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        builder.Services.AddSingleton(Log.Logger);
        builder.Services.AddProblemDetails(ProblemDetailsConfigurationExtension.ConfigureProblemDetails)
            .AddProblemDetailsConventions();

        // TODO update CORS for your env
        builder.Services.AddCorsService("PeakLimsCorsPolicy", builder.Environment);
        builder.OpenTelemetryRegistration(builder.Configuration, "PeakLims");
        builder.Services.AddInfrastructure(builder.Environment, builder.Configuration);
        builder.Services.AddChatClient(chatClientBuilder =>
        {
            var aiOptions = builder.Configuration.GetAiOptions();
            var endpoint = new Uri(aiOptions.Url);
            var modelId = aiOptions.ModelName;
            return chatClientBuilder.UseFunctionInvocation()
                .UseLogging()
                // .UseOpenTelemetry()
                // .UseDistributedCache()
                .Use(new OllamaChatClient(endpoint, modelId));
        });

        builder.Services.AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        builder.Services.AddApiVersioningExtension();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // registers all services that inherit from your base service interface - IPeakLimsScopedService
        builder.Services.AddBoundaryServices(Assembly.GetExecutingAssembly());

        builder.Services.AddMvc();

        builder.Services.AddHealthChecks();
        builder.Services.AddSwaggerExtension(builder.Configuration);
    }

    /// <summary>
    /// Registers all services in the assembly of the given interface.
    /// </summary>
    private static void AddBoundaryServices(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (!assemblies.Any())
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for handlers.");

        foreach (var assembly in assemblies)
        {
            var rules = assembly.GetTypes()
                .Where(x => !x.IsAbstract && x.IsClass && x.GetInterface(nameof(IPeakLimsScopedService)) == typeof(IPeakLimsScopedService));

            foreach (var rule in rules)
            {
                foreach (var @interface in rule.GetInterfaces())
                {
                    services.Add(new ServiceDescriptor(@interface, rule, ServiceLifetime.Scoped));
                }
            }
        }
    }
}