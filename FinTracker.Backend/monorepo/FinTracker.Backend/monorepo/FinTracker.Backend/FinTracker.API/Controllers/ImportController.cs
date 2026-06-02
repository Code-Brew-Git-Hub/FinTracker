using FinTracker.Domain.Dtos.Import;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Interfaces.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace FinTracker.API.Controllers;

[ApiController]
[Route("api/import")]
public class ImportController(IImportService importService,
    IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<FileImportResultDto>>> UploadFile(IFormFileCollection files)
    {
        if (files is null || files.Count == 0)
            return BadRequest(ApiResponse<object>.Fail("Файлы не переданы"));

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

                var result = await importService.ImportAsync(reader, file.FileName);

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
        }

        return Ok(ApiResponse.Ok(results));
    }
}
