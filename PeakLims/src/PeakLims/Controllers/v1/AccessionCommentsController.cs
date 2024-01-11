namespace PeakLims.Controllers.v1;

using PeakLims.Domain.AccessionComments.Features;
using PeakLims.Domain.AccessionComments.Dtos;
using PeakLims.Wrappers;
using PeakLims.Domain;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using Services;

[ApiController]
[Route("api/accessioncomments")]
[ApiVersion("1.0")]
public sealed class AccessionCommentsController(IMediator mediator,
    ICurrentUserService currentUserService) : ControllerBase
{
    /// <summary>
    /// Gets a single AccessionComment by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetAccessionComment")]
    public async Task<ActionResult<AccessionCommentDto>> GetAccessionComment(Guid id)
    {
        var query = new GetAccessionComment.Query(id);
        var queryResponse = await mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Gets accession comments by accession
    /// </summary>
    [Authorize]
    [HttpGet("byAccession/{accessionId:guid}", Name = "GetAccessionCommentView")]
    public async Task<ActionResult<AccessionCommentDto>> GetAccessionCommentView(Guid accessionId)
    {
        var query = new GetAccessionCommentView.Query(accessionId);
        var queryResponse = await mediator.Send(query);
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
        var commandResponse = await mediator.Send(command);

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
        var command = new UpdateAccessionComment.Command(id, accessionComment.Comment, currentUserService.UserIdentifier);
        await mediator.Send(command);
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
        await mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
