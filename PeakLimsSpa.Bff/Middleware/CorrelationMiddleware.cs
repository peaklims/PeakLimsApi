namespace PeakLimsSpa.Bff.Middleware;

public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (!IsCorrelationFailedException(ex)) throw;
            
            context.Response.Redirect("/bff/login");
        }
    }

    private bool IsCorrelationFailedException(Exception ex)
    {
        return ex.InnerException?.Message.Contains("Correlation failed") == true
               || ex.Message.Contains("Correlation failed");
    }
}
