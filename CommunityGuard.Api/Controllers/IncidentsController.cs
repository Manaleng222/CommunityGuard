using CommunityGuard.Api.Services;
using CommunityGuard.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityGuard.Api.Controllers;

[Authorize] // CRITICAL: Requires a valid JWT to access any of these endpoints.
public class IncidentsController : BaseController
{
    private readonly IIncidentService _incidentService;

    public IncidentsController(IIncidentService incidentService)
    {
        _incidentService = incidentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateIncidentRequest request)
    {
        // Pass the user ID from the JWT to the service.
        var incidentId = await _incidentService.CreateAsync(request, CurrentUserId);

        // Return a 201 Created with the location of the new incident.
        return CreatedAtAction(nameof(GetById), new { id = incidentId }, null);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        // Check if the logged-in user is a Controller
        bool isController = User.IsInRole("Controller");

        // Pass the User ID and Role to the service
        var result = await _incidentService.GetPagedAsync(page, pageSize, CurrentUserId, isController);
        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var incident = await _incidentService.GetByIdAsync(id);
        if (incident == null) return NotFound();

        return Ok(incident);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateIncidentStatusRequest request)
    {
        // Call the service to update the status.
        await _incidentService.UpdateStatusAsync(id, request, CurrentUserId);

        // Return 204 No Content (standard for successful updates with no body).
        return NoContent();
    }
}