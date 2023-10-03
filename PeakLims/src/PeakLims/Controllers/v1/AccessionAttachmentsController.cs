namespace PeakLims.Controllers.v1;

using PeakLims.Domain.AccessionAttachments.Features;
using PeakLims.Domain.AccessionAttachments.Dtos;
using PeakLims.Wrappers;
using PeakLims.Domain;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Threading;
using MediatR;

[ApiController]
[Route("api/accessionattachments")]
[ApiVersion("1.0")]
public sealed class AccessionAttachmentsController: ControllerBase
{
    private readonly IMediator _mediator;

    public AccessionAttachmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all AccessionAttachments.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetAccessionAttachments")]
    public async Task<IActionResult> GetAccessionAttachments([FromQuery] AccessionAttachmentParametersDto accessionAttachmentParametersDto)
    {
        var query = new GetAccessionAttachmentList.Query(accessionAttachmentParametersDto);
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
    /// Gets a single AccessionAttachment by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{accessionAttachmentId:guid}", Name = "GetAccessionAttachment")]
    public async Task<ActionResult<AccessionAttachmentDto>> GetAccessionAttachment(Guid accessionAttachmentId)
    {
        var query = new GetAccessionAttachment.Query(accessionAttachmentId);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Uploads a file to an Accession
    /// </summary>
    [Authorize]
    [HttpPost("uploadTo/{accessionId:guid}", Name = "UploadAccessionAttachmentFile")]
    public async Task<IActionResult> UploadAccessionAttachmentFile(Guid accessionId, [FromForm] UploadAccessionAttachmentDto data)
    {
        var command = new UploadAccessionAttachmentFile.Command(accessionId, data.File);
        await _mediator.Send(command);
        return NoContent();
    }
    

    /// <summary>
    /// Updates an entire existing AccessionAttachment.
    /// </summary>
    [Authorize]
    [HttpPut("{accessionAttachmentId:guid}", Name = "UpdateAccessionAttachment")]
    public async Task<IActionResult> UpdateAccessionAttachment(Guid accessionAttachmentId, AccessionAttachmentForUpdateDto accessionAttachment)
    {
        var command = new UpdateAccessionAttachment.Command(accessionAttachmentId, accessionAttachment);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Deletes an existing AccessionAttachment record.
    /// </summary>
    [Authorize]
    [HttpDelete("{accessionAttachmentId:guid}", Name = "DeleteAccessionAttachment")]
    public async Task<ActionResult> DeleteAccessionAttachment(Guid accessionAttachmentId)
    {
        var command = new DeleteAccessionAttachment.Command(accessionAttachmentId);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
