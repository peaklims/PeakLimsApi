namespace PeakLims.Controllers.v1;

using PeakLims.Domain.TestOrders.Features;
using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.Domain;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Threading;
using Domain.Panels.Features;
using Domain.TestOrderCancellationReasons;
using MediatR;
using Asp.Versioning;

[ApiController]
[Route("api/v{v:apiVersion}/testorders")]
[ApiVersion("1.0")]
public sealed class TestOrdersController: ControllerBase
{
    private readonly IMediator _mediator;

    public TestOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all TestOrders.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetTestOrders")]
    public async Task<IActionResult> GetTestOrders([FromQuery] TestOrderParametersDto testOrderParametersDto)
    {
        var query = new GetTestOrderList.Query(testOrderParametersDto);
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
    /// Gets a single TestOrder by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetTestOrder")]
    public async Task<ActionResult<TestOrderDto>> GetTestOrder(Guid id)
    {
        var query = new GetTestOrder.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new TestOrder record.
    /// </summary>
    [Authorize]
    [HttpPost("{accessionId:guid}", Name = "AddTestOrder")]
    public async Task<IActionResult> AddTestOrder([FromRoute] Guid accessionId, [FromBody]TestOrderForCreationDto testOrderForCreation)
    {
        var command = new AddTestOrder.Command(accessionId, testOrderForCreation.TestId);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Gets all orderable panels and tests
    /// </summary>
    [Authorize]
    [HttpGet("orderablePanelsAndTests", Name = "GetOrderablePanelsAndTests")]
    public async Task<IActionResult> GetOrderablePanelsAndTests()
    {
        var query = new GetOrderablePanelsAndTests.Query();
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Sets the sample on a test order
    /// </summary>
    [Authorize]
    [HttpPut("{testOrderId:guid}/setSample/{sampleId}", Name = "SetSampleOnTestOrder")]
    public async Task<IActionResult> SetSampleOnTestOrder([FromRoute] Guid testOrderId, [FromRoute] Guid? sampleId)
    {
        var command = new SetSampleOnTestOrder.Command(testOrderId, sampleId);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Sets the sample on a test order
    /// </summary>
    [Authorize]
    [HttpPut("{testOrderId:guid}/clearSample", Name = "SetSampleOnTestOrderAsCleared")]
    public async Task<IActionResult> SetSampleOnTestOrderAsCleared([FromRoute] Guid testOrderId)
    {
        var command = new SetSampleOnTestOrder.Command(testOrderId, null);
        await _mediator.Send(command);
        return NoContent();
    }
    
    /// <summary>
    /// Cancels a test order
    /// </summary>
    [Authorize]
    [HttpPut("{testOrderId:guid}/cancel", Name = "CancelTestOrder")]
    public async Task<IActionResult> CancelTestOrder([FromRoute] Guid testOrderId, [FromBody] TestOrderCancellationDto testOrderCancellationDto)
    {
        var command = new CancelTestOrder.Command(testOrderId, testOrderCancellationDto.Reason, testOrderCancellationDto.Comments);
        await _mediator.Send(command);
        return NoContent();
    }
    
    /// <summary>
    /// Get a list of all possible cancellation reasons
    /// </summary>
    [Authorize]
    [HttpGet("cancellationReasons", Name = "GetTestOrderCancellationReasons")]
    public async Task<IActionResult> GetCancellationReasons()
        => Ok(CancellationReason.ListNames());

    /// <summary>
    /// Marks a test order as STAT
    /// </summary>
    [Authorize]
    [HttpPut("{testOrderId:guid}/markstat", Name = "MarkTestOrderAsStat")]
    public async Task<IActionResult> MarkTestOrderAsStat([FromRoute] Guid testOrderId)
    {
        var command = new MarkTestOrderAsStat.Command(testOrderId);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Marks a test order as normal priority
    /// </summary>
    [Authorize]
    [HttpPut("{testOrderId:guid}/marknormal", Name = "MarkTestOrderAsNormal")]
    public async Task<IActionResult> MarkTestOrderAsNormal([FromRoute] Guid testOrderId)
    {
        var command = new MarkTestOrderAsNormal.Command(testOrderId);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Adjusts the due date of a test order
    /// </summary>
    [Authorize]
    [HttpPut("{testOrderId:guid}/adjustDueDate", Name = "AdjustTestOrderDueDate")]
    public async Task<IActionResult> AdjustTestOrderDueDate([FromRoute] Guid testOrderId, AdjustDueDateDto request)
    {
        var command = new AdjustTestOrderDueDate.Command(testOrderId, request.DueDate);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
