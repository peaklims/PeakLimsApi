namespace PeakLims.Controllers.v1;

using PeakLims.Domain.HipaaAuditLogs.Features;
using PeakLims.Domain.HipaaAuditLogs.Dtos;
using PeakLims.Resources;
using PeakLims.Domain;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using Asp.Versioning;

[ApiController]
[Route("api/v{v:apiVersion}/hipaaauditlogs")]
[ApiVersion("1.0")]
public sealed class HipaaAuditLogsController: ControllerBase
{
    private readonly IMediator _mediator;

    public HipaaAuditLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all HipaaAuditLogs.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetHipaaAuditLogs")]
    public async Task<IActionResult> GetHipaaAuditLogs([FromQuery] HipaaAuditLogParametersDto hipaaAuditLogParametersDto)
    {
        var query = new GetHipaaAuditLogList.Query(hipaaAuditLogParametersDto);
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
    /// Gets a single HipaaAuditLog by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{hipaaAuditLogId:guid}", Name = "GetHipaaAuditLog")]
    public async Task<ActionResult<HipaaAuditLogDto>> GetHipaaAuditLog(Guid hipaaAuditLogId)
    {
        var query = new GetHipaaAuditLog.Query(hipaaAuditLogId);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Updates an entire existing HipaaAuditLog.
    /// </summary>
    [Authorize]
    [HttpPut("{hipaaAuditLogId:guid}", Name = "UpdateHipaaAuditLog")]
    public async Task<IActionResult> UpdateHipaaAuditLog(Guid hipaaAuditLogId, HipaaAuditLogForUpdateDto hipaaAuditLog)
    {
        var command = new UpdateHipaaAuditLog.Command(hipaaAuditLogId, hipaaAuditLog);
        await _mediator.Send(command);
        return NoContent();
    }


    // endpoint marker - do not delete this comment
}
