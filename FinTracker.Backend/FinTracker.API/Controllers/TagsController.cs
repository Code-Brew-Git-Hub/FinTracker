using FinTracker.Domain.Dtos.Tags;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("/api/tags")]
public class TagsController(ITagService tagService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<TagDto[]>>> GetAll()
    {
        var tags = await tagService.GetAllAsync();

        return Ok(ApiResponse.Ok(tags));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TagDto>>> Create([FromBody] CreateTagDto dto)
    {
        var createdTag = await tagService.CreateAsync(dto.Name);

        return Ok(ApiResponse.Ok(createdTag));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await tagService.DeleteAsync(id);

        return NoContent();
    }
}
