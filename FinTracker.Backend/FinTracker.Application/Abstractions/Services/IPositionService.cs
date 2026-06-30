using FinTracker.Domain.Dtos.Positions;

namespace FinTracker.Application.Abstractions.Services;

public interface IPositionService
{
    Task<List<PositionDto>> GetAllAsync(Guid transactionId);
    Task<PositionDto> CreateAsync(Guid transactionId, CreatePositionDto dto);
    Task<PositionDto> UpdateAsync(Guid transactionId, Guid itemId, UpdatePositionDto dto);
    Task DeleteAsync(Guid transactionId, Guid itemId);
}
