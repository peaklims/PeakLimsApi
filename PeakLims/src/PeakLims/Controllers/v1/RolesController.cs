namespace PeakLims.Controllers.v1;

using Domain;
using Domain.Roles;
using Exceptions;
using HeimGuard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/roles")]
[ApiVersion("1.0")]
public sealed class RolesController: ControllerBase
{
    private readonly IHeimGuardClient _heimGuard;

    public RolesController(IHeimGuardClient heimGuard)
    {
        _heimGuard = heimGuard;
    }

    /// <summary>
    /// Gets a list of all available roles.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetRoles")]
    public List<string> GetRoles()
    {
        return Role.ListNames();
    }
}