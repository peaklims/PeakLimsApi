namespace PeakLims.Controllers.v1;

using PeakLims.Domain.Containers.Features;
using PeakLims.Domain.Containers.Dtos;
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
[Route("api/containers")]
[ApiVersion("1.0")]
public sealed class ContainersController: ControllerBase
{
    private readonly IMediator _mediator;

    public ContainersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all Containers.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetContainers")]
    public async Task<IActionResult> GetContainers([FromQuery] ContainerParametersDto containerParametersDto)
    {
        var query = new GetContainerList.Query(containerParametersDto);
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
    /// Gets a single Container by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetContainer")]
    public async Task<ActionResult<ContainerDto>> GetContainer(Guid id)
    {
        var query = new GetContainer.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new Container record.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddContainer")]
    public async Task<ActionResult<ContainerDto>> AddContainer([FromBody]ContainerForCreationDto containerForCreation)
    {
        var command = new AddContainer.Command(containerForCreation);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetContainer",
            new { commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Updates an entire existing Container.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}", Name = "UpdateContainer")]
    public async Task<IActionResult> UpdateContainer(Guid id, ContainerForUpdateDto container)
    {
        var command = new UpdateContainer.Command(id, container);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Deletes an existing Container record.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}", Name = "DeleteContainer")]
    public async Task<ActionResult> DeleteContainer(Guid id)
    {
        var command = new DeleteContainer.Command(id);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
