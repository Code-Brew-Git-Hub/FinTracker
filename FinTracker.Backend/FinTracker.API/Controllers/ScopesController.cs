using Microsoft.AspNetCore.Mvc;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Dtos.Scopes;
using FinTracker.Domain.Dtos.Transactions;
using FinTracker.Domain.Interfaces.Services;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("/api/scopes")]
public class ScopesController(IScopeService scopeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ScopeDto[]>>> GetAll()
    {
        var scopes = await scopeService.GetAllAsync();

        return Ok(ApiResponse.Ok(scopes));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ScopeDto>>> GetById([FromRoute] Guid id)
    {
        var scope = await scopeService.GetByIdAsync(id);

        return Ok(ApiResponse.Ok(scope));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ScopeDto>>> Create([FromBody] CreateScopeDto dto)
    {
        var createdScope = await scopeService.CreateAsync(dto.Name);

        return Ok(ApiResponse.Ok(createdScope));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ScopeDto>>> Update([FromRoute] Guid id, [FromBody] UpdateScopeDto dto)
    {
        var updatedScope = await scopeService.UpdateAsync(id, dto.Name);

        return Ok(ApiResponse.Ok(updatedScope));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await scopeService.DeleteAsync(id);

        return NoContent();
    }

    [HttpGet("{scopeId:guid}/transactions")]
    public async Task<ActionResult<ApiResponse<TransactionDto[]>>> GetTransactions(Guid scopeId)
    {
        var transactions = await scopeService.GetTransactionsAsync(scopeId);

        return Ok(ApiResponse.Ok(transactions));
    }
}
