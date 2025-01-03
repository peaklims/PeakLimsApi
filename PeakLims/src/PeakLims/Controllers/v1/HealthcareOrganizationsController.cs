namespace PeakLims.Controllers.v1;

using PeakLims.Domain.HealthcareOrganizations.Features;
using PeakLims.Domain.HealthcareOrganizations.Dtos;
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
[Route("api/v{v:apiVersion}/healthcareorganizations")]
[ApiVersion("1.0")]
public sealed class HealthcareOrganizationsController: ControllerBase
{
    private readonly IMediator _mediator;

    public HealthcareOrganizationsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a paginated list of all HealthcareOrganizations.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetHealthcareOrganizations")]
    public async Task<IActionResult> GetHealthcareOrganizations([FromQuery] HealthcareOrganizationParametersDto healthcareOrganizationParametersDto)
    {
        var query = new GetHealthcareOrganizationList.Query(healthcareOrganizationParametersDto);
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
    /// Gets a list of all HealthcareOrganizations.
    /// </summary>
    [Authorize]
    [HttpGet("all", Name = "GetAllHealthcareOrganizations")]
    public async Task<IActionResult> GetAllHealthcareOrganizations()
    {
        var query = new GetAllHealthcareOrganizations.Query();
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Gets a single HealthcareOrganization by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetHealthcareOrganization")]
    public async Task<ActionResult<HealthcareOrganizationDto>> GetHealthcareOrganization(Guid id)
    {
        var query = new GetHealthcareOrganization.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new HealthcareOrganization record.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddHealthcareOrganization")]
    public async Task<ActionResult<HealthcareOrganizationDto>> AddHealthcareOrganization([FromBody]HealthcareOrganizationForCreationDto healthcareOrganizationForCreation)
    {
        var command = new AddHealthcareOrganization.Command(healthcareOrganizationForCreation);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetHealthcareOrganization",
            new { commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Updates an entire existing HealthcareOrganization.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}", Name = "UpdateHealthcareOrganization")]
    public async Task<IActionResult> UpdateHealthcareOrganization(Guid id, HealthcareOrganizationForUpdateDto healthcareOrganization)
    {
        var command = new UpdateHealthcareOrganization.Command(id, healthcareOrganization);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Activates a HealthcareOrganization.
    /// </summary>
    [Authorize]
    [HttpPut("{organizationId:guid}/activate", Name = "ActivateHealthcareOrganization")]
    public async Task<IActionResult> ActivateHealthcareOrganization(Guid organizationId)
    {
        var command = new ActivateHealthcareOrganization.Command(organizationId);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Deactivates a HealthcareOrganization.
    /// </summary>
    [Authorize]
    [HttpPut("{organizationId:guid}/deactivate", Name = "DeactivateHealthcareOrganization")]
    public async Task<IActionResult> DeactivateHealthcareOrganization(Guid organizationId)
    {
        var command = new DeactivateHealthcareOrganization.Command(organizationId);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
