// source: https://github.com/jasontaylordev/CleanArchitecture/blob/main/src/WebUI/Filters/ApiExceptionFilterAttribute.cs

namespace PeakLimsSpa.Bff.Middleware;

using System.ComponentModel.DataAnnotations;
using System.Net;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

public static class ProblemDetailsConfigurationExtension
{ 
    public static void ConfigureProblemDetails(ProblemDetailsOptions options)
    {
        // You can configure the middleware to re-throw certain types of exceptions, all exceptions or based on a predicate.
        // This is useful if you have upstream middleware that needs to do additional handling of exceptions.
        // options.Rethrow<NotSupportedException>();

        options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
        options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

        options.MapAuthenticationFailureException();
        
        // You can configure the middleware to re-throw certain types of exceptions, all exceptions or based on a predicate.
        // This is useful if you have upstream middleware that  needs to do additional handling of exceptions.
        // options.Rethrow<NotSupportedException>();

        // You can configure the middleware to ignore any exceptions of the specified type.
        // This is useful if you have upstream middleware that  needs to do additional handling of exceptions.
        // Note that unlike Rethrow, additional information will not be added to the exception.
        // options.Ignore<DivideByZeroException>();

        // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
        // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
        options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
    }
    
    // For correlation failures after sitting on login page for a very long time
    private static void MapAuthenticationFailureException(this ProblemDetailsOptions options) =>
        options.Map<AuthenticationFailureException>((ctx, ex) =>
        {
            // Check if the exception message contains "Correlation failed"
            if (ex.Message.Contains("Correlation failed"))
            {
                ctx.Response.Redirect("/bff/login");
                ctx.Response.StatusCode = (int)HttpStatusCode.Found;
                return null; 
            }
            else
            {
                // For other AuthenticationFailureExceptions, create a ProblemDetails response
                var factory = ctx.RequestServices.GetRequiredService<ProblemDetailsFactory>();

                var problemDetails = factory.CreateProblemDetails(
                    ctx,
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Authentication Failure",
                    detail: ex.Message);

                return problemDetails;
            }
        });
}
