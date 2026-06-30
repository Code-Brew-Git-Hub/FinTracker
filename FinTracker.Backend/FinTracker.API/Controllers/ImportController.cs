using System.Text.Json;
using FinTracker.Application.Abstractions.Services;
using FinTracker.Domain.Dtos.Import;
using FinTracker.Domain.Dtos.Universal;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("api/import")]
public class ImportController(IImportService importService) : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [HttpPost("preview")]
    public async Task<ActionResult<ApiResponse<CsvPreviewDto>>> Preview(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(ApiResponse<object>.Fail("Файл не передан или пустой"));

        try
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            var preview = await importService.PreviewAsync(reader, file.FileName);
            return Ok(ApiResponse.Ok(preview));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<FileImportResultDto>>> UploadFile(
        IFormFileCollection files,
        [FromForm] Guid? presetId,
        [FromForm] string? mapping)
    {
        if (files is null || files.Count == 0)
            return BadRequest(ApiResponse<object>.Fail("Файлы не переданы"));

        if (!presetId.HasValue && string.IsNullOrWhiteSpace(mapping))
            return BadRequest(ApiResponse<object>.Fail("Укажите presetId или mapping"));

        CsvParseOptionsDto? parseOptions = null;
        if (!string.IsNullOrWhiteSpace(mapping))
        {
            try
            {
                parseOptions = JsonSerializer.Deserialize<CsvParseOptionsDto>(mapping, JsonOptions);
            }
            catch (JsonException)
            {
                return BadRequest(ApiResponse<object>.Fail("Некорректный JSON в поле mapping"));
            }

            if (parseOptions?.ColumnMapping is null)
                return BadRequest(ApiResponse<object>.Fail("В mapping должно быть указано поле columnMapping"));
        }

        var results = new List<FileImportResultDto>();

        foreach (var file in files)
        {
            if (file.Length == 0)
            {
                results.Add(new FileImportResultDto
                {
                    FileName = file.FileName,
                    Success = false,
                    Error = "Файл пустой"
                });
                continue;
            }

            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                var result = parseOptions != null
                    ? await importService.ImportAsync(reader, file.FileName, parseOptions)
                    : await importService.ImportAsync(reader, file.FileName, presetId!.Value);

                results.Add(new FileImportResultDto
                {
                    FileName = file.FileName,
                    Success = true,
                    Result = result
                });
            }
            catch (ArgumentException ex)
            {
                results.Add(new FileImportResultDto
                {
                    FileName = file.FileName,
                    Success = false,
                    Error = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                results.Add(new FileImportResultDto
                {
                    FileName = file.FileName,
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        return Ok(ApiResponse.Ok(results));
    }
}
