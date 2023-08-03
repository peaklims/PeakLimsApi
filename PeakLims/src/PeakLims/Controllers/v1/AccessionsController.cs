namespace PeakLims.Controllers.v1;

using PeakLims.Domain.Accessions.Features;
using PeakLims.Domain.Accessions.Dtos;
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
[Route("api/accessions")]
[ApiVersion("1.0")]
public sealed class AccessionsController: ControllerBase
{
    private readonly IMediator _mediator;

    public AccessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all Accessions.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetAccessions")]
    public async Task<IActionResult> GetAccessions([FromQuery] AccessionParametersDto accessionParametersDto)
    {
        var query = new GetAccessionList.Query(accessionParametersDto);
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
    /// Gets a single Accession by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetAccession")]
    public async Task<ActionResult<AccessionDto>> GetAccession(Guid id)
    {
        var query = new GetAccession.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new Accession record.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddAccession")]
    public async Task<ActionResult<AccessionDto>> AddAccession()
    {
        var command = new AddAccession.Command();
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetAccession",
            new { commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Deletes an existing Accession record.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}", Name = "DeleteAccession")]
    public async Task<ActionResult> DeleteAccession(Guid id)
    {
        var command = new DeleteAccession.Command(id);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
