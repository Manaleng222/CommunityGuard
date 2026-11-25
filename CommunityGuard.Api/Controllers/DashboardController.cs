using CommunityGuard.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityGuard.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class DashboardController : BaseController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        // This controller has one job: return the aggregated stats.
        var stats = await _dashboardService.GetStatsAsync();
        return Ok(stats);
    }
}
