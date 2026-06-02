namespace FinTracker.Domain.Dtos.Positions;

public class PositionDto
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public Guid? CategoryId { get; set; }
}
