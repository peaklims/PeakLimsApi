namespace PeakLims.Resources.HangfireUtilities;

using Hangfire.Client;
using Hangfire.Common;

public class JobUserFilterAttribute : JobFilterAttribute, IClientFilter
{
    public void OnCreating(CreatingContext context)
    {
        var user = "job-user-346f9812-16da-4a72-9db2-f066661d6593";
        context.SetJobParameter("User", user);
    }

    public void OnCreated(CreatedContext context)
    {
    }
}