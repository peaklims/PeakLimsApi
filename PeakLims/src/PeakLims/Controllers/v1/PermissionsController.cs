namespace PeakLims.Controllers.v1;

using Domain;
using HeimGuard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;

[ApiController]
[Route("api/permissions")]
[ApiVersion("1.0")]
public sealed class PermissionsController: ControllerBase
{
    private readonly IHeimGuardClient _heimGuard;
    private readonly IUserPolicyHandler _userPolicyHandler;

    public PermissionsController(IHeimGuardClient heimGuard, IUserPolicyHandler userPolicyHandler)
    {
        _heimGuard = heimGuard;
        _userPolicyHandler = userPolicyHandler;
    }

    /// <summary>
    /// Gets a list of all available permissions.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetPermissions")]
    public List<string> GetPermissions()
    {
        _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanGetPermissions);
        return Permissions.List();
    }

    /// <summary>
    /// Gets a list of the current user's assigned permissions.
    /// </summary>
    [Authorize]
    [HttpGet("mine", Name = "GetAssignedPermissions")]
    public async Task<List<string>> GetAssignedPermissions()
    {
        var permissions = await _userPolicyHandler.GetUserPermissions();
        return permissions.ToList();
    }
}