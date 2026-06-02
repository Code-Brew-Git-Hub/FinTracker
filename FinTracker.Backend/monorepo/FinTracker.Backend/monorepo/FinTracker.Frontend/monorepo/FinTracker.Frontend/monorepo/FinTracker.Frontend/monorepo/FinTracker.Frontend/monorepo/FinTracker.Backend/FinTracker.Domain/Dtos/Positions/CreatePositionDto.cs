namespace FinTracker.Domain.Dtos.Positions;

public class CreatePositionDto
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public Guid? CategoryId { get; set; }
}
