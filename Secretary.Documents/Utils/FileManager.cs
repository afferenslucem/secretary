using System;
using System.IO;
using Secretary.Logging;
using Serilog;

namespace Secretary.Documents.utils;

public class FileManager : IFileManager
{
    private ILogger _logger = LogPoint.GetLogger<FileManager>();
    public void DeleteFile(string? filePath)
    {
        try
        {
            _logger.Debug($"Try to delete file {filePath}");
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.Debug($"{filePath} successfully deleted");
            }
            else
            {
                _logger.Warning($"{filePath ?? "null"} file didn't exists");
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, $"Could not delete {filePath}");
        }
    }
    
    public string CreateTempFile()
    {
        return Retry(() => TryCreateTempFile(), 3);
    }
    
    private string TryCreateTempFile()
    {
        var tempFileName = Path.GetTempFileName();
        File.Move(tempFileName, tempFileName.Replace(".tmp", ".docx"));
        tempFileName = tempFileName.Replace(".tmp", ".docx");

        return tempFileName;
    }

    private string Retry(Func<string> action, int times)
    {
        try
        {
            return action();
        }
        catch (Exception e)
        {
            if (times <= 1) throw;

            return Retry(action, times - 1);
        }
    }
}