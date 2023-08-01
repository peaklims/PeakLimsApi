namespace PeakLims.Controllers.v1;

using PeakLims.Domain.HealthcareOrganizationContacts.Features;
using PeakLims.Domain.HealthcareOrganizationContacts.Dtos;
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
[Route("api/healthcareorganizationcontacts")]
[ApiVersion("1.0")]
public sealed class HealthcareOrganizationContactsController: ControllerBase
{
    private readonly IMediator _mediator;

    public HealthcareOrganizationContactsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all HealthcareOrganizationContacts.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetHealthcareOrganizationContacts")]
    public async Task<IActionResult> GetHealthcareOrganizationContacts([FromQuery] HealthcareOrganizationContactParametersDto healthcareOrganizationContactParametersDto)
    {
        var query = new GetHealthcareOrganizationContactList.Query(healthcareOrganizationContactParametersDto);
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
    /// Gets a single HealthcareOrganizationContact by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetHealthcareOrganizationContact")]
    public async Task<ActionResult<HealthcareOrganizationContactDto>> GetHealthcareOrganizationContact(Guid id)
    {
        var query = new GetHealthcareOrganizationContact.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new HealthcareOrganizationContact record.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddHealthcareOrganizationContact")]
    public async Task<ActionResult<HealthcareOrganizationContactDto>> AddHealthcareOrganizationContact([FromBody]HealthcareOrganizationContactForCreationDto healthcareOrganizationContactForCreation)
    {
        var command = new AddHealthcareOrganizationContact.Command(healthcareOrganizationContactForCreation);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetHealthcareOrganizationContact",
            new { commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Updates an entire existing HealthcareOrganizationContact.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}", Name = "UpdateHealthcareOrganizationContact")]
    public async Task<IActionResult> UpdateHealthcareOrganizationContact(Guid id, HealthcareOrganizationContactForUpdateDto healthcareOrganizationContact)
    {
        var command = new UpdateHealthcareOrganizationContact.Command(id, healthcareOrganizationContact);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Deletes an existing HealthcareOrganizationContact record.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}", Name = "DeleteHealthcareOrganizationContact")]
    public async Task<ActionResult> DeleteHealthcareOrganizationContact(Guid id)
    {
        var command = new DeleteHealthcareOrganizationContact.Command(id);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
