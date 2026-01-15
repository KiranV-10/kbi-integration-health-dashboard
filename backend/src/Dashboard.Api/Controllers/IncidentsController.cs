using Dashboard.Application.Models;
using Dashboard.Application.Services.Interfaces;
using Dashboard.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Api.Controllers;

[ApiController]
[Route("api/incidents")]
public class IncidentsController : ControllerBase
{
    private readonly IIncidentService _incidentService;

    public IncidentsController(IIncidentService incidentService)
    {
        _incidentService = incidentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetIncidents(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        IncidentStatus? parsedStatus = null;

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<IncidentStatus>(status, true, out var result))
            {
                return BadRequest(new { message = "Invalid status value." });
            }

            parsedStatus = result;
        }

        var incidents = await _incidentService.GetIncidentsAsync(parsedStatus, cancellationToken);
        return Ok(incidents);
    }

    [HttpPost]
    public async Task<IActionResult> CreateIncident(
        [FromBody] IncidentCreateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var incident = await _incidentService.CreateIncidentAsync(request, cancellationToken);
            return Ok(incident);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:long}/status")]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] long id,
        [FromBody] IncidentStatusUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var incident = await _incidentService.UpdateStatusAsync(id, request, cancellationToken);
        if (incident is null)
        {
            return NotFound();
        }

        return Ok(incident);
    }

    [HttpPut("{id:long}/resolve")]
    public async Task<IActionResult> Resolve(
        [FromRoute] long id,
        [FromBody] IncidentResolveRequest request,
        CancellationToken cancellationToken)
    {
        var incident = await _incidentService.ResolveAsync(id, request, cancellationToken);
        if (incident is null)
        {
            return NotFound();
        }

        return Ok(incident);
    }
}
