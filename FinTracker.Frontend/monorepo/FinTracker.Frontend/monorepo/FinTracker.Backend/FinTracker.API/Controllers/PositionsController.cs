using FinTracker.Domain.Dtos.Positions;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Interfaces.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("api/transactions/{transactionId:guid}/items")]
public class PositionsController(IPositionService itemService,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PositionDto[]>>> GetAll([FromRoute] Guid transactionId)
    {
        var items = await itemService.GetAllAsync(transactionId);

        var itemsDto = mapper.Map<PositionDto[]>(items);

        return Ok(ApiResponse.Ok(itemsDto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PositionDto>>> Create([FromRoute] Guid transactionId, [FromBody] CreatePositionDto dto)
    {
        var createdItem = await itemService.CreateAsync(transactionId, dto);

        var createdItemDto = mapper.Map<PositionDto>(createdItem);

        return Ok(ApiResponse.Ok(createdItemDto));
    }

    [HttpPut("{itemId:guid}")]
    public async Task<ActionResult<ApiResponse<PositionDto>>> Update([FromRoute] Guid transactionId, [FromRoute] Guid itemId, [FromBody] UpdatePositionDto dto)
    {
        var updatedItem = await itemService.UpdateAsync(transactionId, itemId, dto);

        var updatedItemDto = mapper.Map<PositionDto>(updatedItem);

        return Ok(ApiResponse.Ok(updatedItemDto));
    }

    [HttpDelete("{itemId:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid transactionId, [FromRoute] Guid itemId)
    {
        await itemService.DeleteAsync(transactionId, itemId);

        return NoContent();
    }
}