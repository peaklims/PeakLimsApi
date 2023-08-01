namespace PeakLims.Controllers.v1;

using PeakLims.Domain.Samples.Features;
using PeakLims.Domain.Samples.Dtos;
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
[Route("api/samples")]
[ApiVersion("1.0")]
public sealed class SamplesController: ControllerBase
{
    private readonly IMediator _mediator;

    public SamplesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all Samples.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetSamples")]
    public async Task<IActionResult> GetSamples([FromQuery] SampleParametersDto sampleParametersDto)
    {
        var query = new GetSampleList.Query(sampleParametersDto);
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
    /// Gets a single Sample by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetSample")]
    public async Task<ActionResult<SampleDto>> GetSample(Guid id)
    {
        var query = new GetSample.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new Sample record.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddSample")]
    public async Task<ActionResult<SampleDto>> AddSample([FromBody]SampleForCreationDto sampleForCreation)
    {
        var command = new AddSample.Command(sampleForCreation);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetSample",
            new { commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Updates an entire existing Sample.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}", Name = "UpdateSample")]
    public async Task<IActionResult> UpdateSample(Guid id, SampleForUpdateDto sample)
    {
        var command = new UpdateSample.Command(id, sample);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Deletes an existing Sample record.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}", Name = "DeleteSample")]
    public async Task<ActionResult> DeleteSample(Guid id)
    {
        var command = new DeleteSample.Command(id);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
