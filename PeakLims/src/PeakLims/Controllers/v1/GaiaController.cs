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
using Domain.Gaia.Features.Generators;
using Microsoft.Extensions.AI;

[ApiController]
[Route("api/v{v:apiVersion}/gaia")]
[ApiVersion("1.0")]
public sealed class GaiaController(IChatClient chatClient) : ControllerBase
{
    /// <summary>
    /// Test Org Creation
    /// </summary>
    // [Authorize]
    [HttpPost(Name = "TryOrgs")]
    public async Task<IActionResult> TryOrgs()
    {
        var organizations = new OrganizationGenerator(chatClient).GenerateCoreAsync();
        var orgsToReturn = new List<OrganizationResponse>();
        await foreach (var org in organizations)
        {
            orgsToReturn.Add(org);
        }
        return Ok(orgsToReturn);
    }
}
