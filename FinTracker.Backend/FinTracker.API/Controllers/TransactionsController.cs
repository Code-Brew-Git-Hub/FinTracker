using FinTracker.Data.Services;
using FinTracker.Domain.Dtos.Transactions;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Models.ModelsToHelp;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionsController(ITransactionService transactionService,
    IMapper mapper) : ControllerBase
{

    /// <summary>
    /// Список транзакций с фильтрацией и пагинацией (total, page, pageSize, totalPages).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<TransactionDto>>>> GetAll(
        [FromQuery] TransactionFilter filter,
        [FromQuery] bool includeDeleted = false)
    {
        var result = await transactionService.GetFilteredAsync(filter, includeDeleted);

        return Ok(ApiResponse.Ok(new PagedResponse<TransactionDto>
        {
            Items = mapper.Map<List<TransactionDto>>(result.Items),
            Total = result.Total,
            Page = result.Page,
            PageSize = result.PageSize
        }));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TransactionDto>>> GetById([FromRoute] Guid id, [FromQuery] bool includeDeleted = false)
    {
        var transaction = await transactionService.GetByIdAsync(id, includeDeleted);

        var transactionDto = mapper.Map<TransactionDto>(transaction);

        return Ok(ApiResponse.Ok(transactionDto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TransactionDto>>> Create([FromBody] CreateTransactionDto dto)
    {
        var createdTransaction = await transactionService.CreateAsync(dto);

        var createdTransactionDto = mapper.Map<TransactionDto>(createdTransaction);

        return Ok(ApiResponse.Ok(createdTransactionDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<TransactionDto>>> Update([FromRoute] Guid id, [FromBody] UpdateTransactionDto dto)
    {
        var updatedTransaction = await transactionService.UpdateAsync(id, dto);

        var updatedTransactionDto = mapper.Map<TransactionDto>(updatedTransaction);

        return Ok(ApiResponse.Ok(updatedTransactionDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> SoftDelete([FromRoute] Guid id)
    {
        await transactionService.SoftDeleteAsync(id);

        return NoContent();
    }

    [HttpPatch("bulk")]
    public async Task<ActionResult> BulkUpdate([FromBody] BulkUpdateDto dto)
    {
        await transactionService.BulkUpdateAsync(dto);

        return NoContent();
    }
}
