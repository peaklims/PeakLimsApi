namespace PeakLims.Controllers.v1;

using PeakLims.Domain.Tests.Features;
using PeakLims.Domain.Tests.Dtos;
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
[Route("api/tests")]
[ApiVersion("1.0")]
public sealed class TestsController: ControllerBase
{
    private readonly IMediator _mediator;

    public TestsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all Tests.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetTests")]
    public async Task<IActionResult> GetTests([FromQuery] TestParametersDto testParametersDto)
    {
        var query = new GetTestList.Query(testParametersDto);
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
    /// Gets a single Test by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetTest")]
    public async Task<ActionResult<TestDto>> GetTest(Guid id)
    {
        var query = new GetTest.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new Test record.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddTest")]
    public async Task<ActionResult<TestDto>> AddTest([FromBody]TestForCreationDto testForCreation)
    {
        var command = new AddTest.Command(testForCreation);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetTest",
            new { commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Updates an entire existing Test.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}", Name = "UpdateTest")]
    public async Task<IActionResult> UpdateTest(Guid id, TestForUpdateDto test)
    {
        var command = new UpdateTest.Command(id, test);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Deletes an existing Test record.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}", Name = "DeleteTest")]
    public async Task<ActionResult> DeleteTest(Guid id)
    {
        var command = new DeleteTest.Command(id);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
