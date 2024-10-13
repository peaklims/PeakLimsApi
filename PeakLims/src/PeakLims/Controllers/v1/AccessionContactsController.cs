namespace PeakLims.Controllers.v1;

using PeakLims.Domain.AccessionContacts.Features;
using PeakLims.Domain.AccessionContacts.Dtos;
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
[Route("api/v{v:apiVersion}/accessioncontacts")]
[ApiVersion("1.0")]
public sealed class AccessionContactsController: ControllerBase
{
    private readonly IMediator _mediator;

    public AccessionContactsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all AccessionContacts.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetAccessionContacts")]
    public async Task<IActionResult> GetAccessionContacts([FromQuery] AccessionContactParametersDto accessionContactParametersDto)
    {
        var query = new GetAccessionContactList.Query(accessionContactParametersDto);
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
    /// Gets a list of contacts for a particular accession
    /// </summary>
    [Authorize]
    [HttpGet("byAccession/{accessionId:guid}", Name = "GetContactsForAnAccession")]
    public async Task<IActionResult> GetContactsForAnAccession(Guid accessionId)
    {
        var query = new GetContactsForAnAccession.Query(accessionId);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Gets a single AccessionContact by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetAccessionContact")]
    public async Task<ActionResult<AccessionContactDto>> GetAccessionContact(Guid id)
    {
        var query = new GetAccessionContact.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }

    // endpoint marker - do not delete this comment
}
