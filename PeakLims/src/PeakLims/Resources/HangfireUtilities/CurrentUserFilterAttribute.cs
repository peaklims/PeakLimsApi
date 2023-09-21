namespace PeakLims.Resources.HangfireUtilities;

using Hangfire.Client;
using Hangfire.Common;

public class CurrentUserFilterAttribute : JobFilterAttribute, IClientFilter
{
    public void OnCreating(CreatingContext context)
    {
        var argue = context.Job.Args.FirstOrDefault(x => x is IJobWithUserContext);
        if (argue == null)
            throw new Exception($"This job does not implement the {nameof(IJobWithUserContext)} interface");

        var jobParameters = argue as IJobWithUserContext;
        var user = jobParameters?.User;

        if(user == null)
            throw new Exception($"A User could not be established");

        context.SetJobParameter("User", user);
    }

    public void OnCreated(CreatedContext context)
    {
    }
}