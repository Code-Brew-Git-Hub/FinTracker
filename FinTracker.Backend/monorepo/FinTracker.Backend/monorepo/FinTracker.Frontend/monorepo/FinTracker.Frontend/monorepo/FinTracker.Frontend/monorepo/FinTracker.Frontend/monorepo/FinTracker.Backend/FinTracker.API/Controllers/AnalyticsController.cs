using FinTracker.Domain.Dtos.Analytics;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Enums;
using FinTracker.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<AnalyticsSummaryDto>>> GetSummary([FromQuery] AnalyticsFilterDto filter)
    {
        var summary = await analyticsService.GetSummaryAsync(filter);
        return Ok(ApiResponse.Ok(summary));
    }

    [HttpGet("by-category")]
    public async Task<ActionResult<ApiResponse<CategoryStatDto[]>>> GetByCategory([FromQuery] AnalyticsFilterDto filter)
    {
        var stats = await analyticsService.GetByCategoryAsync(filter);
        return Ok(ApiResponse.Ok(stats));
    }

    [HttpGet("by-scope")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ScopeStatDto>>>> GetByScope([FromQuery] AnalyticsFilterDto filter)
    {
        var stats = await analyticsService.GetByScopeAsync(filter);
        return Ok(ApiResponse.Ok(stats));
    }

    [HttpGet("by-tag")]
    public async Task<ActionResult<ApiResponse<TagStatDto[]>>> GetByTag([FromQuery] AnalyticsFilterDto filter)
    {
        var stats = await analyticsService.GetByTagAsync(filter);
        return Ok(ApiResponse.Ok(stats));
    }

    [HttpGet("by-time")]
    public async Task<ActionResult<ApiResponse<TimeStatDto[]>>> GetByTime(
        [FromQuery] AnalyticsFilterDto filter,
        [FromQuery] TimeGrouping grouping = TimeGrouping.Month)
    {
        var stats = await analyticsService.GetByTimeAsync(filter, grouping);
        return Ok(ApiResponse.Ok(stats));
    }
}
