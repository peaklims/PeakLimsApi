namespace PeakLims.Controllers.v1;

using Domain;
using Domain.Roles;
using Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

[ApiController]
[Route("api/v{v:apiVersion}/roles")]
[ApiVersion("1.0")]
public sealed class RolesController() : ControllerBase
{
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