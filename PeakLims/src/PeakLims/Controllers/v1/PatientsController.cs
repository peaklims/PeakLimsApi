namespace PeakLims.Controllers.v1;

using PeakLims.Domain.Patients.Features;
using PeakLims.Domain.Patients.Dtos;
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
[Route("api/patients")]
[ApiVersion("1.0")]
public sealed class PatientsController: ControllerBase
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Gets a list of all Patients.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetPatients")]
    public async Task<IActionResult> GetPatients([FromQuery] PatientParametersDto patientParametersDto)
    {
        var query = new GetPatientList.Query(patientParametersDto);
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
    /// Gets a single Patient by ID.
    /// </summary>
    [Authorize]
    [HttpGet("{id:guid}", Name = "GetPatient")]
    public async Task<ActionResult<PatientDto>> GetPatient(Guid id)
    {
        var query = new GetPatient.Query(id);
        var queryResponse = await _mediator.Send(query);
        return Ok(queryResponse);
    }


    /// <summary>
    /// Creates a new Patient record.
    /// </summary>
    [Authorize]
    [HttpPost(Name = "AddPatient")]
    public async Task<ActionResult<PatientDto>> AddPatient([FromBody]PatientForCreationDto patientForCreation)
    {
        var command = new AddPatient.Command(patientForCreation);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetPatient",
            new { commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Updates an entire existing Patient.
    /// </summary>
    [Authorize]
    [HttpPut("{id:guid}", Name = "UpdatePatient")]
    public async Task<IActionResult> UpdatePatient(Guid id, PatientForUpdateDto patient)
    {
        var command = new UpdatePatient.Command(id, patient);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Deletes an existing Patient record.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}", Name = "DeletePatient")]
    public async Task<ActionResult> DeletePatient(Guid id)
    {
        var command = new DeletePatient.Command(id);
        await _mediator.Send(command);
        return NoContent();
    }

    // endpoint marker - do not delete this comment
}
