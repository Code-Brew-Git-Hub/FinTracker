using System.ComponentModel.DataAnnotations;

namespace FinTracker.Domain.Dtos.Tags;

public class CreateTagDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
}