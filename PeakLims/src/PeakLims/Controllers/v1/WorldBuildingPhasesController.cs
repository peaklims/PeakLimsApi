namespace PeakLims.Controllers.v1;

using PeakLims.Domain.WorldBuildingPhases.Features;
using PeakLims.Domain.WorldBuildingPhases.Dtos;
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
[Route("api/v{v:apiVersion}/worldbuildingphases")]
[ApiVersion("1.0")]
public sealed class WorldBuildingPhasesController(IMediator mediator): ControllerBase
{    

    /// <summary>
    /// Gets a list of all WorldBuildingPhases.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetWorldBuildingPhases")]
    public async Task<IActionResult> GetWorldBuildingPhases([FromQuery] WorldBuildingPhaseParametersDto worldBuildingPhaseParametersDto)
    {
        var query = new GetWorldBuildingPhaseList.Query(worldBuildingPhaseParametersDto);
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
    /// Gets a single WorldBuildingPhase by ID.
    /// </summary>
    [HttpGet("{worldBuildingPhaseId:guid}", Name = "GetWorldBuildingPhase")]
    public async Task<ActionResult<WorldBuildingPhaseDto>> GetWorldBuildingPhase(Guid worldBuildingPhaseId)
    {
        var query = new GetWorldBuildingPhase.Query(worldBuildingPhaseId);
        var queryResponse = await mediator.Send(query);
        return Ok(queryResponse);
    }

    // endpoint marker - do not delete this comment
}
