namespace PeakLims.Services;

using System.Security.Claims;
using Resources;
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
    Guid? OrganizationId { get; }
    Guid GetOrganizationId();
    bool IsHangfire { get; }
    bool HasCrossOrganizationContext { get; }
}

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor, IJobContextAccessor jobContextAccessor)
    : ICurrentUserService
{
    public ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User ?? CreatePrincipalFromJobContextUserId();
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
    public bool IsHangfire => jobContextAccessor?.UserContext?.User != null;
    public bool HasCrossOrganizationContext => User?.IsInRole(Consts.CrossOrganizationContext) ?? false;
    public Guid? OrganizationId
    {
        get
        {
            var organizationId = User?.FindFirstValue("organization_id");
            var hasTenantId = Guid.TryParse(organizationId, out var result);
            return hasTenantId ? result : null;
        }
    }
    public Guid GetOrganizationId() => OrganizationId ?? throw new InvalidOperationException("Invalid organization id (null)");
    
    private ClaimsPrincipal? CreatePrincipalFromJobContextUserId()
    {
        var userId = jobContextAccessor?.UserContext?.User;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        var identity = new ClaimsIdentity(claims, $"hangfirejob-{userId}");
        if (userId == Consts.SuperHangfireUser)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, Consts.CrossOrganizationContext));
        }
        return new ClaimsPrincipal(identity);
    }
}