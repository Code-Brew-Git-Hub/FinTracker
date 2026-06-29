using FinTracker.Domain.Dtos.Positions;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("api/transactions/{transactionId:guid}/items")]
public class PositionsController(IPositionService itemService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PositionDto[]>>> GetAll([FromRoute] Guid transactionId)
    {
        var items = await itemService.GetAllAsync(transactionId);

        return Ok(ApiResponse.Ok(items));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PositionDto>>> Create([FromRoute] Guid transactionId, [FromBody] CreatePositionDto dto)
    {
        var createdItem = await itemService.CreateAsync(transactionId, dto);

        return Ok(ApiResponse.Ok(createdItem));
    }

    [HttpPut("{itemId:guid}")]
    public async Task<ActionResult<ApiResponse<PositionDto>>> Update([FromRoute] Guid transactionId, [FromRoute] Guid itemId, [FromBody] UpdatePositionDto dto)
    {
        var updatedItem = await itemService.UpdateAsync(transactionId, itemId, dto);

        return Ok(ApiResponse.Ok(updatedItem));
    }

    [HttpDelete("{itemId:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid transactionId, [FromRoute] Guid itemId)
    {
        await itemService.DeleteAsync(transactionId, itemId);

        return NoContent();
    }
}