namespace PeakLims.Controllers.v1;

using PeakLims.Domain.RolePermissions.Features;
using PeakLims.Domain.RolePermissions.Dtos;
using PeakLims.Wrappers;
using PeakLims.Domain;
using SharedKernel.Domain;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Threading;
using MediatR;

[ApiController]
[Route("api/rolepermissions")]
[ApiVersion("1.0")]
public sealed class RolePermissionsController: ControllerBase
{
    private readonly IMediator _mediator;

    public RolePermissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all RolePermissions.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetRolePermissions")]
    public async Task<IActionResult> GetRolePermissions([FromQuery] RolePermissionParametersDto rolePermissionParametersDto)
    {
        var query = new GetRolePermissionList.Query(rolePermissionParametersDto);
        var queryResponse = await _mediator.Send(query);

        var paginationMetadata = new
        {
            totalCount = queryResponse.TotalCount,
            pageSize = queryResponse.PageSize,
            currentPageSize = queryResponse.CurrentPageSize,
            currentStartIndex = queryResponse.CurrentStartIndex,
            currentEndIndex = queryResponse.CurrentEndIndex,
            pageNumber = queryResponse.PageNumber,
            totalPages = queryResponse.TotalPages,
            hasPrevious = queryResponse.HasPrevious,
            hasNext = queryResponse.HasNext
        };

        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));

        return Ok(queryResponse);
    }


    /// <summary>
    /// Gets a single RolePermission by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetRolePermission")]
    public async Task<ActionResult<RolePermissionDto>> GetRolePermission(Guid id)
    {
        var query = new GetRolePermission.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new RolePermission record.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddRolePermission")]
    public async Task<ActionResult<RolePermissionDto>> AddRolePermission([FromBody]RolePermissionForCreationDto rolePermissionForCreation)
    {
        var command = new AddRolePermission.Command(rolePermissionForCreation);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetRolePermission",
            new { commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Updates an entire existing RolePermission.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}", Name = "UpdateRolePermission")]
    public async Task<IActionResult> UpdateRolePermission(Guid id, RolePermissionForUpdateDto rolePermission)
    {
        var command = new UpdateRolePermission.Command(id, rolePermission);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Deletes an existing RolePermission record.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}", Name = "DeleteRolePermission")]
    public async Task<ActionResult> DeleteRolePermission(Guid id)
    {
        var command = new DeleteRolePermission.Command(id);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
