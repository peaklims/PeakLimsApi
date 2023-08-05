namespace PeakLims.Controllers.v1;

using PeakLims.Domain.TestOrders.Features;
using PeakLims.Domain.TestOrders.Dtos;
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
[Route("api/testorders")]
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
    [HttpPost(Name = "AddTestOrder")]
    public async Task<ActionResult<TestOrderDto>> AddTestOrder([FromBody]TestOrderForCreationDto testOrderForCreation)
    {
        var command = new AddTestOrder.Command(testOrderForCreation.TestId, testOrderForCreation.PanelId);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetTestOrder",
            new { commandResponse.Id },
            commandResponse);
    }

    // endpoint marker - do not delete this comment
}
