namespace PeakLims.Controllers.v1;

using PeakLims.Domain.AccessionContacts.Features;
using PeakLims.Domain.AccessionContacts.Dtos;
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
[Route("api/accessioncontacts")]
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


    /// <summary>
    /// Creates a new AccessionContact record.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddAccessionContact")]
    public async Task<ActionResult<AccessionContactDto>> AddAccessionContact([FromBody]AccessionContactForCreationDto accessionContactForCreation)
    {
        var command = new AddAccessionContact.Command(accessionContactForCreation);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetAccessionContact",
            new { commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Updates an entire existing AccessionContact.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}", Name = "UpdateAccessionContact")]
    public async Task<IActionResult> UpdateAccessionContact(Guid id, AccessionContactForUpdateDto accessionContact)
    {
        var command = new UpdateAccessionContact.Command(id, accessionContact);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Deletes an existing AccessionContact record.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}", Name = "DeleteAccessionContact")]
    public async Task<ActionResult> DeleteAccessionContact(Guid id)
    {
        var command = new DeleteAccessionContact.Command(id);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
