using System;
using System.IO;

namespace Secretary.Documents.utils;

public class FileManager
{
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