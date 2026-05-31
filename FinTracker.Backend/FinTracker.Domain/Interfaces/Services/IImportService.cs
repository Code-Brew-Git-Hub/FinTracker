using FinTracker.Domain.Dtos.Import;

namespace FinTracker.Domain.Interfaces.Services;

public interface IImportService
{
    Task<ImportResultDto> ImportAsync(StreamReader reader, string filename);
}
