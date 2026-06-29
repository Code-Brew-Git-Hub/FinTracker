using FinTracker.Domain.Dtos.Positions;
using FinTracker.Domain.Models;

namespace FinTracker.Domain.Interfaces.Services;

public interface IPositionService
{
    Task<List<PositionDto>> GetAllAsync(Guid transactionId);
    Task<PositionDto> CreateAsync(Guid transactionId, CreatePositionDto dto);
    Task<PositionDto> UpdateAsync(Guid transactionId, Guid itemId, UpdatePositionDto dto);
    Task DeleteAsync(Guid transactionId, Guid itemId);
}
