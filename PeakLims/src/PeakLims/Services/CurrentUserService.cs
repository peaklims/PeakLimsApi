namespace PeakLims.Services;

using System.Security.Claims;
using Resources.HangfireUtilities;

public interface ICurrentUserService : IPeakLimsScopedService
{
    ClaimsPrincipal? User { get; }
    string? UserIdentifier { get; }
    string? Email { get; }
    string? FirstName { get; }
    string? LastName { get; }
    string? Username { get; }
    string? ClientId { get; }
    bool IsMachine { get; }
}

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJobContextAccessor _jobContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IJobContextAccessor jobContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _jobContextAccessor = jobContextAccessor;
    }

    public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User ?? CreatePrincipalFromJobContextUserId();
    public string? UserIdentifier => User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? Email => User?.FindFirstValue(ClaimTypes.Email);
    public string? FirstName => User?.FindFirstValue(ClaimTypes.GivenName);
    public string? LastName => User?.FindFirstValue(ClaimTypes.Surname);
    public string? Username => User
        ?.Claims
        ?.FirstOrDefault(x => x.Type is "preferred_username" or "username")
        ?.Value;
    public string? ClientId => User
        ?.Claims
        ?.FirstOrDefault(x => x.Type is "client_id" or "clientId")
        ?.Value;
    public bool IsMachine => ClientId != null;
    
    private ClaimsPrincipal? CreatePrincipalFromJobContextUserId()
    {
        var userId = _jobContextAccessor?.UserContext?.User;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        var identity = new ClaimsIdentity(claims, $"hangfirejob-{userId}");
        return new ClaimsPrincipal(identity);
    }
}