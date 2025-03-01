namespace PeakLims.Controllers.v1;

using PeakLims.Domain.PanelOrders.Features;
using PeakLims.Domain.PanelOrders.Dtos;
using PeakLims.Domain;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Threading;
using Domain.Accessions.Features;
using MediatR;
using Asp.Versioning;
using Domain.TestOrderCancellationReasons;

[ApiController]
[Route("api/v{v:apiVersion}/panelorders")]
[ApiVersion("1.0")]
public sealed class PanelOrdersController: ControllerBase
{
    private readonly IMediator _mediator;

    public PanelOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all PanelOrders.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetPanelOrders")]
    public async Task<IActionResult> GetPanelOrders([FromQuery] PanelOrderParametersDto panelOrderParametersDto)
    {
        var query = new GetPanelOrderList.Query(panelOrderParametersDto);
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
    /// Gets a single PanelOrder by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{panelOrderId:guid}", Name = "GetPanelOrder")]
    public async Task<ActionResult<PanelOrderDto>> GetPanelOrder(Guid panelOrderId)
    {
        var query = new GetPanelOrder.Query(panelOrderId);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }

    /// <summary>
    /// Deletes an existing PanelOrder record.
    /// </summary>
    [Authorize]
    [HttpDelete("{panelOrderId:guid}", Name = "DeletePanelOrder")]
    public async Task<ActionResult> DeletePanelOrder(Guid panelOrderId)
    {
        var command = new DeletePanelOrder.Command(panelOrderId);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Cancels a panel order.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}/cancel", Name = "CancelPanelOrder")]
    public async Task<IActionResult> CancelPanelOrder(Guid id, [FromBody] TestOrderCancellationDto cancellationDto)
    {
        var command = new CancelPanelOrder.Command(id, cancellationDto.Reason, cancellationDto.Comments);
        await _mediator.Send(command);
        return NoContent();
    }
    
    /// <summary>
    /// Get a list of all possible cancellation reasons
    /// </summary>
    [Authorize]
    [HttpGet("cancellationReasons", Name = "GetPanelOrderCancellationReasons")]
    public async Task<IActionResult> GetCancellationReasons()
        => Ok(CancellationReason.ListNames());

    // endpoint marker - do not delete this comment
}
