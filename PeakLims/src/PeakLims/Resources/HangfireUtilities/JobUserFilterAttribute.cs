namespace PeakLims.Resources.HangfireUtilities;

using Hangfire.Client;
using Hangfire.Common;

public class JobUserFilterAttribute : JobFilterAttribute, IClientFilter
{
    public void OnCreating(CreatingContext context)
    {
        var user = Consts.SuperHangfireUser;
        context.SetJobParameter("User", user);
    }

    public void OnCreated(CreatedContext context)
    {
    }
}