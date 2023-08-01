namespace PeakLims.Controllers.v1;

using PeakLims.Domain.Panels.Features;
using PeakLims.Domain.Panels.Dtos;
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
[Route("api/panels")]
[ApiVersion("1.0")]
public sealed class PanelsController: ControllerBase
{
    private readonly IMediator _mediator;

    public PanelsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all Panels.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetPanels")]
    public async Task<IActionResult> GetPanels([FromQuery] PanelParametersDto panelParametersDto)
    {
        var query = new GetPanelList.Query(panelParametersDto);
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
    /// Gets a single Panel by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetPanel")]
    public async Task<ActionResult<PanelDto>> GetPanel(Guid id)
    {
        var query = new GetPanel.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new Panel record.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddPanel")]
    public async Task<ActionResult<PanelDto>> AddPanel([FromBody]PanelForCreationDto panelForCreation)
    {
        var command = new AddPanel.Command(panelForCreation);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetPanel",
            new { commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Updates an entire existing Panel.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}", Name = "UpdatePanel")]
    public async Task<IActionResult> UpdatePanel(Guid id, PanelForUpdateDto panel)
    {
        var command = new UpdatePanel.Command(id, panel);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Deletes an existing Panel record.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}", Name = "DeletePanel")]
    public async Task<ActionResult> DeletePanel(Guid id)
    {
        var command = new DeletePanel.Command(id);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
