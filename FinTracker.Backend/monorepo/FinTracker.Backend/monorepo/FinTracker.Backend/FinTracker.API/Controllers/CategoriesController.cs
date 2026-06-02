using FinTracker.Domain.Dtos.Categories;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Interfaces.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(ICategoryService categoryService,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<CategoryDto[]>>> GetAll()
    {
        var categories = await categoryService.GetAllAsync();

        var dto = mapper.Map<CategoryDto[]>(categories);

        return Ok(ApiResponse.Ok(dto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById([FromRoute] Guid id)
    {
        var category = await categoryService.GetByIdAsync(id);

        var dto = mapper.Map<CategoryDto>(category);

        return Ok(ApiResponse.Ok(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create([FromBody] CreateCategoryDto dto)
    {
        var createdCategory = await categoryService.CreateAsync(dto.Name);

        var catDto = mapper.Map<CategoryDto>(createdCategory);

        return Ok(ApiResponse.Ok(catDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update([FromRoute] Guid id, [FromBody] UpdateCategoryDto dto)
    {

        var updatedCategory = await categoryService.UpdateAsync(id, dto.Name);

        var catDto = mapper.Map<CategoryDto>(updatedCategory);

        return Ok(ApiResponse.Ok(catDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await categoryService.DeleteAsync(id);
        return NoContent();
    }
}
