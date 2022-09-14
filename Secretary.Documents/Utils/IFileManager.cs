using System;
using System.IO;
using Secretary.Logging;
using Serilog;

namespace Secretary.Documents.utils;

public interface IFileManager
{
    void DeleteFile(string? filePath);

    string CreateTempFile();
}