namespace PeakLims.Controllers.v1;

using PeakLims.Domain.Patients.Features;
using PeakLims.Domain.Patients.Dtos;
using PeakLims.Domain;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using Asp.Versioning;
using Domain.PatientRelationships.Dtos;
using Domain.PatientRelationships.Features;

[ApiController]
[Route("api/v{v:apiVersion}/patients")]
[ApiVersion("1.0")]
public sealed class PatientsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets a list of all Patients.
    /// </summary>
    [Authorize]
    [HttpGet(Name = "GetPatients")]
    public async Task<IActionResult> GetPatients([FromQuery] PatientParametersDto patientParametersDto)
    {
        var query = new GetPatientList.Query(patientParametersDto);
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

        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));

        return Ok(queryResponse);
    }

    /// <summary>
    /// Search for an existing patient
    /// </summary>
    [Authorize]
    [HttpGet("searchExistingPatients", Name = "SearchExistingPatients")]
    public async Task<IActionResult> SearchExistingPatients([FromQuery] PatientParametersDto patientParametersDto)
    {
        var query = new SearchExistingPatients.Query(patientParametersDto);
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
        var queryResponse = await mediator.Send(query);
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
        var commandResponse = await mediator.Send(command);

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
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Adds a patient relationship
    /// </summary>
    [Authorize]
    [HttpPost("relationship", Name = "AddRelationship")]
    public async Task<IActionResult> AddRelationship([FromBody]AddPatientRelationshipDto relationship)
    {
        var command = new AddRelationship.Command(relationship);
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Gets a list of patient relationships
    /// </summary>
    [Authorize]
    [HttpGet("{patientId:guid}/relationships", Name = "GetPatientRelationships")]
    public async Task<IActionResult> GetPatientRelationships(Guid patientId)
    {
        var command = new GetPatientRelationships.Query(patientId);
        var response = await mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Deletes a relationship
    /// </summary>
    [Authorize]
    [HttpDelete("relationship/{relationshipId:guid}", Name = "RemoveRelationship")]
    public async Task<IActionResult> RemoveRelationship(Guid relationshipId)
    {
        var command = new RemoveRelationship.Command(relationshipId);
        await mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Gets a suggested relationship
    /// </summary>
    [Authorize]
    [HttpPost("relationship/suggested", Name = "GetSuggestedMatchingRole")]
    public async Task<IActionResult> GetSuggestedMatchingRole(GetSuggestedMatchingRoleRequestDto getSuggestedMatchingRoleRequestDto)
    {
        var command = new GetSuggestedMatchingRole.Command(getSuggestedMatchingRoleRequestDto);
        return Ok(await mediator.Send(command));
    }
    
    
    // endpoint marker - do not delete this comment
}
