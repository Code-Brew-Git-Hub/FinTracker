using Microsoft.AspNetCore.Mvc;
using FinTracker.Data.Services;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Dtos.Scopes;
using MapsterMapper;
using FinTracker.Domain.Dtos.Transactions;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("/api/scopes")]
public class ScopesController(IScopeService scopeService,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ScopeDto[]>>> GetAll()
    {
        var scopes = await scopeService.GetAllAsync();

        var scopesDto = mapper.Map<ScopeDto[]>(scopes);

        return Ok(ApiResponse.Ok(scopesDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ScopeDto>>> GetById([FromRoute] Guid id)
    {
        var scope = await scopeService.GetByIdAsync(id);

        var scopeDto = mapper.Map<ScopeDto>(scope);

        return Ok(ApiResponse.Ok(scopeDto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ScopeDto>>> Create([FromBody] CreateScopeDto dto)
    {
        var createdScope = await scopeService.CreateAsync(dto.Name);

        var createdScopeDto = mapper.Map<ScopeDto>(createdScope);

        return Ok(ApiResponse.Ok(createdScopeDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ScopeDto>>> Update([FromRoute] Guid id, [FromBody] UpdateScopeDto dto)
    {
        var updatedScope = await scopeService.UpdateAsync(id, dto.Name);

        var updatedScopeDto = mapper.Map<ScopeDto>(updatedScope);

        return Ok(ApiResponse.Ok(updatedScopeDto));
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

        var transactionsDto = mapper.Map<TransactionDto[]>(transactions);

        return Ok(ApiResponse.Ok(transactionsDto));
    }
}
