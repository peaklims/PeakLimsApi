namespace PeakLims.Controllers.v1;

using PeakLims.Domain.Accessions.Features;
using PeakLims.Domain.Accessions.Dtos;
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
[Route("api/accessions")]
[ApiVersion("1.0")]
public sealed class AccessionsController: ControllerBase
{
    private readonly IMediator _mediator;

    public AccessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all Accessions.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetAccessions")]
    public async Task<IActionResult> GetAccessions([FromQuery] AccessionParametersDto accessionParametersDto)
    {
        var query = new GetAccessionList.Query(accessionParametersDto);
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
    /// Gets a list of all Accessions for the worklist view.
    /// </summary>
    [Authorize]
    [HttpGet("worklist", Name = "GetAccessioningWorklist")]
    public async Task<IActionResult> GetAccessioningWorklist([FromQuery] AccessionParametersDto accessionParametersDto)
    {
        var query = new GetAccessionWorklist.Query(accessionParametersDto);
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
    /// Gets a single Accession by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetAccession")]
    public async Task<ActionResult<AccessionDto>> GetAccession(Guid id)
    {
        var query = new GetAccession.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Gets a single Accession by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}/forAggregateEdit", Name = "GetEditableAccessionAggregate")]
    public async Task<IActionResult> GetEditableAccessionAggregate(Guid id)
    {
        var query = new GetEditableAccessionAggregate.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }

    /// <summary>
    /// Creates a new Accession record.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddAccession")]
    public async Task<ActionResult<AccessionDto>> AddAccession()
    {
        var command = new AddAccession.Command();
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetAccession",
            new { commandResponse.Id },
            commandResponse);
    }

    /// <summary>
    /// Sets the patient on an accession.
    /// </summary>
    [Authorize]
    [HttpPut("setPatient", Name = "SetAccessionPatient")]
    public async Task<IActionResult> SetAccessionPatient([FromBody] SetPatientDto setPatientDto)
    {
        var command = new SetAccessionPatient.Command(setPatientDto.AccessionId, setPatientDto.PatientId, setPatientDto.PatientForCreation);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Removes the patient from a given accession.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}/removePatient", Name = "RemoveAccessionPatient")]
    public async Task<IActionResult> RemoveAccessionPatient(Guid id)
    {
        var command = new RemoveAccessionPatient.Command(id);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Adds the contact to an accession.
    /// </summary>
    [Authorize]
    [HttpPost("{accessionId:guid}/addContact/{orgContactId:guid}", Name = "AddContactToAccession")]
    public async Task<IActionResult> AddContactToAccession(Guid accessionId, Guid orgContactId)
    {
        var command = new AddContactToAccession.Command(accessionId, orgContactId);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetAccessionContact",
            new { commandResponse.Id },
            commandResponse);
    }

    /// <summary>
    /// Removes the contact from a given accession.
    /// </summary>
    [Authorize]
    [HttpPut("{accessionId:guid}/removeContact/{accessionContactId:guid}", Name= "RemoveContactFromAccession")]
    public async Task<IActionResult> RemoveContactFromAccession(Guid accessionId, Guid accessionContactId)
    {
        var command = new RemoveContactFromAccession.Command(accessionId, accessionContactId);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Sets the organzation on an accession.
    /// </summary>
    [Authorize]
    [HttpPut("{accessionId:guid}/setOrganization/{orgId:guid}", Name = "SetAccessionHealthcareOrganization")]
    public async Task<IActionResult> SetAccessionHealthcareOrganization(Guid accessionId, Guid orgId)
    {
        var command = new SetAccessionHealthcareOrganization.Command(accessionId, orgId);
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Deletes an existing Accession record.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}", Name = "DeleteAccession")]
    public async Task<ActionResult> DeleteAccession(Guid id)
    {
        var command = new DeleteAccession.Command(id);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Removes a given panel order from an accession
    /// </summary>
    [Authorize]
    [HttpPut("{accessionId:guid}/removePanelOrder/{panelOrderId:guid}", Name = "RemovePanelOrderFromAccession")]
    public async Task<IActionResult> RemovePanelOrderFromAccession(Guid accessionId, Guid panelOrderId)
    {
        var command = new RemovePanelOrderFromAccession.Command(accessionId, panelOrderId);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Removes a given test order from an accession
    /// </summary>
    [Authorize]
    [HttpPut("{accessionId:guid}/removeTestOrder/{testOrderId:guid}", Name = "RemoveTestOrderFromAccession")]
    public async Task<IActionResult> RemoveTestOrderFromAccession(Guid accessionId, Guid testOrderId)
    {
        var command = new RemoveTestOrderFromAccession.Command(accessionId, testOrderId);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
