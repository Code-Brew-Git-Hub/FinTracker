using FinTracker.Domain.Dtos.TransactionLinks;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Interfaces.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("api/links")]
public class LinksController(ILinkService linkService, IMapper mapper) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TransactionLinkDto>>> GetById([FromRoute] Guid id)
    {
        var link = await linkService.GetByIdAsync(id);

        var linkDto = mapper.Map<TransactionLinkDto>(link);

        return Ok(ApiResponse.Ok(linkDto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TransactionLinkDto>>> Create([FromBody] CreateTransactionLinkDto dto)
    {
        var createdLink = await linkService.CreateAsync(dto);

        var createdLinkDto = mapper.Map<TransactionLinkDto>(createdLink);

        return Ok(ApiResponse.Ok(createdLinkDto));
    }

    [HttpPost("{id:guid}/transactions")]
    public async Task<ActionResult<ApiResponse<TransactionLinkDto>>> AddTransaction([FromRoute] Guid id, [FromBody] AddTransactionToLinkDto dto)
    {
        var link = await linkService.AddTransactionAsync(id, dto.TransactionId);

        var linkDto = mapper.Map<TransactionLinkDto>(link);

        return Ok(ApiResponse.Ok(linkDto));
    }

    [HttpDelete("{id:guid}/transactions/{transactionId:guid}")]
    public async Task<ActionResult> RemoveTransaction([FromRoute] Guid id, [FromRoute] Guid transactionId)
    {
        await linkService.RemoveTransactionAsync(id, transactionId);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await linkService.DeleteAsync(id);

        return NoContent();
    }
}
