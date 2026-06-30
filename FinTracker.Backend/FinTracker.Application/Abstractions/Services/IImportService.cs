using FinTracker.Domain.Dtos.Import;

namespace FinTracker.Application.Abstractions.Services;

public interface IImportService
{
    Task<CsvPreviewDto> PreviewAsync(StreamReader reader, string filename);
    Task<ImportResultDto> ImportAsync(StreamReader reader, string filename, Guid presetId);
    Task<ImportResultDto> ImportAsync(StreamReader reader, string filename, CsvParseOptionsDto options);
}
