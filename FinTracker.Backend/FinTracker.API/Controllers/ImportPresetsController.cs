using FinTracker.Domain.Dtos.Import;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("api/import/presets")]
public class ImportPresetsController(IImportPresetService importPresetService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ImportPresetListItemDto>>>> GetAll()
    {
        var presets = await importPresetService.GetAllAsync();
        return Ok(ApiResponse.Ok(presets));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ImportPresetDto>>> GetById([FromRoute] Guid id)
    {
        var preset = await importPresetService.GetByIdAsync(id);
        return Ok(ApiResponse.Ok(preset));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ImportPresetDto>>> Create([FromBody] SaveImportPresetDto dto)
    {
        var preset = await importPresetService.CreateAsync(dto);
        return Ok(ApiResponse.Ok(preset));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ImportPresetDto>>> Update(
        [FromRoute] Guid id,
        [FromBody] SaveImportPresetDto dto)
    {
        var preset = await importPresetService.UpdateAsync(id, dto);
        return Ok(ApiResponse.Ok(preset));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await importPresetService.DeleteAsync(id);
        return NoContent();
    }
}
