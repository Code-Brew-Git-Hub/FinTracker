using System.ComponentModel.DataAnnotations;

namespace FinTracker.Domain.Dtos.Scopes;

public class UpdateScopeDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
}
