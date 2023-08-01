namespace PeakLims.Services;

using System.Security.Claims;

public interface ICurrentUserService : IPeakLimsScopedService
{
    ClaimsPrincipal? User { get; }
    string? UserId { get; }
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

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    public string? FirstName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.GivenName);
    public string? LastName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Surname);
    public string? Username => _httpContextAccessor.HttpContext
        ?.User
        ?.Claims
        ?.FirstOrDefault(x => x.Type is "preferred_username" or "username")
        ?.Value;
    public string? ClientId => _httpContextAccessor.HttpContext
        ?.User
        ?.Claims
        ?.FirstOrDefault(x => x.Type is "client_id" or "clientId")
        ?.Value;
    public bool IsMachine => ClientId != null;
}