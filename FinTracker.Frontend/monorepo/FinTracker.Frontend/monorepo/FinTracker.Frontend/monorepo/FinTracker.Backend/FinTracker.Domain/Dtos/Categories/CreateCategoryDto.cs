using System.ComponentModel.DataAnnotations;

namespace FinTracker.Domain.Dtos.Categories;

public class CreateCategoryDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
}
