namespace PeakLims.Controllers.v1;

using PeakLims.Domain.Containers.Features;
using PeakLims.Domain.Containers.Dtos;
using PeakLims.Domain;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using Asp.Versioning;
using Domain.Gaia.Features;
using Domain.Gaia.Features.Generators;
using Domain.HealthcareOrganizations;
using Domain.HealthcareOrganizations.Models;
using Microsoft.Extensions.AI;
using Serilog;

[ApiController]
[Route("api/v{v:apiVersion}/gaia")]
[ApiVersion("1.0")]
public sealed class GaiaController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Test Org Creation
    /// </summary>
    // [Authorize]
    [HttpPost(Name = "AssembleAWorld")]
    public async Task<IActionResult> AssembleAWorld(string specialOrgRequest)
    {
        var command = new AssembleAWorld.Command(specialOrgRequest);
        var worldBuildingAttemptId = await mediator.Send(command);
        return Accepted(worldBuildingAttemptId);
    }
}
