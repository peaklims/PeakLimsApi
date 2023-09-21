namespace PeakLims.Resources.HangfireUtilities;

using System.Security.Claims;
using Hangfire;
using Hangfire.Annotations;
using Hangfire.AspNetCore;
using Hangfire.Client;
using Hangfire.Common;
using Services;

public interface IJobWithUserContext
{
    public string? User { get; set; }
}
public class JobWithUserContext : IJobWithUserContext
{
    public string? User { get; set; }
}
public interface IJobContextAccessor
{
    JobWithUserContext? UserContext { get; set; }
}
public class JobContextAccessor : IJobContextAccessor
{
    public JobWithUserContext? UserContext { get; set; }
}

public class JobWithUserContextActivator : AspNetCoreJobActivator
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public JobWithUserContextActivator([NotNull] IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    public override JobActivatorScope BeginScope(JobActivatorContext context)
    {
        var user = context.GetJobParameter<string>("User");

        if (user == null)
        {
            return base.BeginScope(context);
        }

        var serviceScope = _serviceScopeFactory.CreateScope();

        var userContextForJob = serviceScope.ServiceProvider.GetRequiredService<IJobContextAccessor>();
        userContextForJob.UserContext = new JobWithUserContext {User = user};

        return new ServiceJobActivatorScope(serviceScope);
    }
}