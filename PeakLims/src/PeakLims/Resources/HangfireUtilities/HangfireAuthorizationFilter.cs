namespace PeakLims.Resources.HangfireUtilities;

using Hangfire.Dashboard;

public class HangfireAuthorizationFilter : IDashboardAsyncAuthorizationFilter
{
    private readonly IServiceProvider _serviceProvider;
    
    public HangfireAuthorizationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<bool> AuthorizeAsync(DashboardContext context)
    {
        // TODO alt -- add login handling with cookie handling
        // var heimGuard = _serviceProvider.GetService<IHeimGuardClient>();
        // return await heimGuard.HasPermissionAsync(Permissions.HangfireAccess);

        var env = _serviceProvider.GetService<IWebHostEnvironment>();
        return Task.FromResult(env.IsDevelopment());
    }
}
