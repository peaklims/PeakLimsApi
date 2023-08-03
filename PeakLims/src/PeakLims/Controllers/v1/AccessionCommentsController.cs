namespace PeakLims.Controllers.v1;

using PeakLims.Domain.AccessionComments.Features;
using PeakLims.Domain.AccessionComments.Dtos;
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
[Route("api/accessioncomments")]
[ApiVersion("1.0")]
public sealed class AccessionCommentsController: ControllerBase
{
    private readonly IMediator _mediator;

    public AccessionCommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    /// <summary>
    /// Gets a single AccessionComment by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetAccessionComment")]
    public async Task<ActionResult<AccessionCommentDto>> GetAccessionComment(Guid id)
    {
        var query = new GetAccessionComment.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new AccessionComment record.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddAccessionComment")]
    public async Task<ActionResult<AccessionCommentDto>> AddAccessionComment([FromBody]AccessionCommentForCreationDto accessionCommentForCreation)
    {
        var command = new AddAccessionComment.Command(accessionCommentForCreation.AccessionId, accessionCommentForCreation.Comment);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetAccessionComment",
            new { commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Updates an entire existing AccessionComment.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}", Name = "UpdateAccessionComment")]
    public async Task<IActionResult> UpdateAccessionComment(Guid id, AccessionCommentForUpdateDto accessionComment)
    {
        var command = new UpdateAccessionComment.Command(id, accessionComment.Comment);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Deletes an existing AccessionComment record.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}", Name = "DeleteAccessionComment")]
    public async Task<ActionResult> DeleteAccessionComment(Guid id)
    {
        var command = new DeleteAccessionComment.Command(id);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
