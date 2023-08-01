namespace PeakLims.UnitTests.ServiceTests;

using PeakLims.Services;
using System.Security.Claims;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

public class CurrentUserServiceTests
{
    [Fact]
    public void returns_user_in_context_if_present()
    {
        var name = new Faker().Person.UserName;

        var id = new ClaimsIdentity();
        id.AddClaim(new Claim(ClaimTypes.NameIdentifier, name));

        var context = new DefaultHttpContext().HttpContext;
        context.User = new ClaimsPrincipal(id);

        var sub = Substitute.For<IHttpContextAccessor>();
        sub.HttpContext.Returns(context);
        
        var currentUserService = new CurrentUserService(sub);

        currentUserService.UserId.Should().Be(name);
    }
    
    [Fact]
    public void returns_null_if_user_is_not_present()
    {
        var context = new DefaultHttpContext().HttpContext;
        var sub = Substitute.For<IHttpContextAccessor>();
        sub.HttpContext.Returns(context);
        
        var currentUserService = new CurrentUserService(sub);

        currentUserService.UserId.Should().BeNullOrEmpty();
    }
}