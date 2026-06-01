using FinTracker.Domain.Dtos.Tags;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Interfaces.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("/api/tags")]
public class TagsController(ITagService tagService,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<TagDto[]>>> GetAll()
    {
        var tags = await tagService.GetAllAsync();

        var tagsDto = mapper.Map<TagDto[]>(tags);

        return Ok(ApiResponse.Ok(tagsDto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TagDto>>> Create([FromBody] CreateTagDto dto)
    {
        var createdTag = await tagService.CreateAsync(dto.Name);

        var createdTagDto = mapper.Map<TagDto>(createdTag);

        return Ok(ApiResponse.Ok(createdTagDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await tagService.DeleteAsync(id);

        return NoContent();
    }
}
