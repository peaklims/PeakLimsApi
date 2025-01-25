namespace PeakLims.Controllers.v1;

using PeakLims.Domain.WorldBuildingAttempts.Features;
using PeakLims.Domain.WorldBuildingAttempts.Dtos;
using PeakLims.Resources;
using PeakLims.Domain;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Threading;
using Asp.Versioning;
using MediatR;

[ApiController]
[Route("api/v{v:apiVersion}/worldbuildingattempts")]
[ApiVersion("1.0")]
public sealed class WorldBuildingAttemptsController(IMediator mediator): ControllerBase
{    

    /// <summary>
    /// Gets a list of all WorldBuildingAttempts.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetWorldBuildingAttempts")]
    public async Task<IActionResult> GetWorldBuildingAttempts([FromQuery] WorldBuildingAttemptParametersDto worldBuildingAttemptParametersDto)
    {
        var query = new GetWorldBuildingAttemptList.Query(worldBuildingAttemptParametersDto);
        var queryResponse = await mediator.Send(query);

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

        Response.Headers.Append("X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));

        return Ok(queryResponse);
    }


    /// <summary>
    /// Gets a single WorldBuildingAttempt by ID.
    /// </summary>
    [HttpGet("{worldBuildingAttemptId:guid}", Name = "GetWorldBuildingAttempt")]
    public async Task<ActionResult<WorldBuildingAttemptDto>> GetWorldBuildingAttempt(Guid worldBuildingAttemptId)
    {
        var query = new GetWorldBuildingAttempt.Query(worldBuildingAttemptId);
        var queryResponse = await mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new WorldBuildingAttempt record.
    /// </summary>
    [HttpPost(Name = "AddWorldBuildingAttempt")]
    public async Task<ActionResult<WorldBuildingAttemptDto>> AddWorldBuildingAttempt([FromBody]WorldBuildingAttemptForCreationDto worldBuildingAttemptForCreation)
    {
        var command = new AddWorldBuildingAttempt.Command(worldBuildingAttemptForCreation);
        var commandResponse = await mediator.Send(command);

        return CreatedAtRoute("GetWorldBuildingAttempt",
            new { worldBuildingAttemptId = commandResponse.Id },
            commandResponse);
    }

    // endpoint marker - do not delete this comment
}
