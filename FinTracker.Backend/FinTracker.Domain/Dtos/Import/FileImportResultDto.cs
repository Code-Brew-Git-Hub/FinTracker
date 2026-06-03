namespace FinTracker.Domain.Dtos.Import;

public class FileImportResultDto
{
    public string FileName { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
    public ImportResultDto? Result { get; set; }
}
