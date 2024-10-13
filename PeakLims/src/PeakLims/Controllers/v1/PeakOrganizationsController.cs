namespace PeakLims.Controllers.v1;

using PeakLims.Domain.PeakOrganizations.Features;
using PeakLims.Domain.PeakOrganizations.Dtos;
using PeakLims.Resources;
using PeakLims.Domain;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Threading;
using Asp.Versioning;
using MediatR;

[ApiController]
[Route("api/v{v:apiVersion}/peakorganizations")]
[ApiVersion("1.0")]
public sealed class PeakOrganizationsController(IMediator mediator): ControllerBase
{    

    /// <summary>
    /// Gets a list of all PeakOrganizations.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetPeakOrganizations")]
    public async Task<IActionResult> GetPeakOrganizations([FromQuery] PeakOrganizationParametersDto peakOrganizationParametersDto)
    {
        var query = new GetPeakOrganizationList.Query(peakOrganizationParametersDto);
        var queryResponse = await mediator.Send(query);

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

        Response.Headers.Append("X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));

        return Ok(queryResponse);
    }


    /// <summary>
    /// Gets a single PeakOrganization by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{peakOrganizationId:guid}", Name = "GetPeakOrganization")]
    public async Task<ActionResult<PeakOrganizationDto>> GetPeakOrganization(Guid peakOrganizationId)
    {
        var query = new GetPeakOrganization.Query(peakOrganizationId);
        var queryResponse = await mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new PeakOrganization record.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddPeakOrganization")]
    public async Task<ActionResult<PeakOrganizationDto>> AddPeakOrganization([FromBody]PeakOrganizationForCreationDto peakOrganizationForCreation)
    {
        var command = new AddPeakOrganization.Command(peakOrganizationForCreation);
        var commandResponse = await mediator.Send(command);

        return CreatedAtRoute("GetPeakOrganization",
            new { peakOrganizationId = commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Updates an entire existing PeakOrganization.
    /// </summary>
    [Authorize]
    [HttpPut("{peakOrganizationId:guid}", Name = "UpdatePeakOrganization")]
    public async Task<IActionResult> UpdatePeakOrganization(Guid peakOrganizationId, PeakOrganizationForUpdateDto peakOrganization)
    {
        var command = new UpdatePeakOrganization.Command(peakOrganizationId, peakOrganization);
        await mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
