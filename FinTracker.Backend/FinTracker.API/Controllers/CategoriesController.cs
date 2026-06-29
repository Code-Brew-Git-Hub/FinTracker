using FinTracker.Domain.Dtos.Categories;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAll()
    {
        var categories = await categoryService.GetAllAsync();

        return Ok(ApiResponse.Ok(categories));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById([FromRoute] Guid id)
    {
        var category = await categoryService.GetByIdAsync(id);

        return Ok(ApiResponse.Ok(category));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create([FromBody] CreateCategoryDto dto)
    {
        var createdCategory = await categoryService.CreateAsync(dto.Name);

        return Ok(ApiResponse.Ok(createdCategory));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update([FromRoute] Guid id, [FromBody] UpdateCategoryDto dto)
    {

        var updatedCategory = await categoryService.UpdateAsync(id, dto.Name);

        return Ok(ApiResponse.Ok(updatedCategory));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await categoryService.DeleteAsync(id);
        return NoContent();
    }
}
