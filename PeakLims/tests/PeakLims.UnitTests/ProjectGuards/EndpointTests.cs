namespace PeakLims.UnitTests.ProjectGuards;

using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using TestHelpers;
using Xunit;

public sealed class EndpointTests
{
    /// <summary>
    /// Turn on this test to check that all endpoints are protected by an Authorize attribute.
    /// </summary>
    // [Fact]
    public void can_protect_all_endpoints_except_opt_outs()
    {
        // Arrange
        var endpoints = GetEndpointsFromProject().ToList();
        var unprotectedEndpoints = new List<string>()
        {
            // Add endpoints that are deliberately not protected here
        };
        endpoints = endpoints.Where(x => !unprotectedEndpoints.Contains(x.Name)).ToList();

        // Act
        var unauthenticatedEndpoints = endpoints.Where(e => !e.RequiresAuthentication);

        // Assert
        unauthenticatedEndpoints.Should().BeEmpty("All endpoints should require authorization, unless explicitly exempt.");
    }

    private static IEnumerable<Endpoint> GetEndpointsFromProject()
    {
        var apiAssembly = UnitTestUtils.GetApiAssembly();
        var controllers = apiAssembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Controller)) || t.IsSubclassOf(typeof(ControllerBase)));

        var endpoints = controllers.SelectMany(controller => controller.GetMethods())
            .Where(method => method.IsPublic && method.IsDefined(typeof(HttpMethodAttribute)));

        return endpoints.Select(endpoint => new Endpoint
        {
            Name = endpoint.Name,
            RequiresAuthentication = endpoint.IsDefined(typeof(AuthorizeAttribute))
        });
    }

    private sealed class Endpoint
    {
        public string Name { get; set; }
        public bool RequiresAuthentication { get; set; }
    }
}