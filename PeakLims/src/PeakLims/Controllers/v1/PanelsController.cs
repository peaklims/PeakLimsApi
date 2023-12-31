namespace PeakLims.Controllers.v1;

using PeakLims.Domain.Panels.Features;
using PeakLims.Domain.Panels.Dtos;
using PeakLims.Wrappers;
using PeakLims.Domain;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Threading;
using Domain.Accessions.Features;
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
    /// Adds a test to a panel
    /// </summary>
    [Authorize]
    [HttpPut("{panelId:guid}/addTest/{testId:guid}", Name = "AddTestToPanel")]
    public async Task<IActionResult> AddTestToPanel(Guid panelId, Guid testId)
    {
        var command = new AddTestToPanel.Command(panelId, testId);
        await _mediator.Send(command);
        return NoContent();
    }
    

    /// <summary>
    /// Removes a test from a panel
    /// </summary>
    [Authorize]
    [HttpPut("{panelId:guid}/removeTest/{testId:guid}", Name = "RemoveTestToPanel")]
    public async Task<IActionResult> RemoveTestToPanel(Guid panelId, Guid testId)
    {
        var command = new RemoveTestFromPanel.Command(panelId, testId);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Activates a panel
    /// </summary>
    [Authorize]
    [HttpPut("{panelId:guid}/activate", Name = "ActivatePanel")]
    public async Task<IActionResult> ActivatePanel(Guid panelId)
    {
        var command = new ActivatePanel.Command(panelId);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Deactivates a panel
    /// </summary>
    [Authorize]
    [HttpPut("{panelId:guid}/deactivate", Name = "DeactivatePanel")]
    public async Task<IActionResult> DeactivatePanel(Guid panelId)
    {
        var command = new DeactivatePanel.Command(panelId);
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


    /// <summary>
    /// Adds a panel to an accession
    /// </summary>
    [Authorize]
    [HttpPost("toAccession", Name = "AddPanelToAccession")]
    public async Task<IActionResult> AddPanelToAccession([FromQuery] Guid accessionId, [FromQuery] Guid panelId)
    {
        var command = new AddPanelToAccession.Command(accessionId, panelId);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
