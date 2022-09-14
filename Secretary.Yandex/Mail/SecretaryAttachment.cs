﻿using MimeKit;

namespace Secretary.Yandex.Mail;

public class SecretaryAttachment
{
    public SecretaryAttachment(string path, string fileName, ContentType contentType)
    {
        Path = path;
        FileName = fileName;
        ContentType = contentType;
    }

    public string Path { get; set; }
    public string FileName { get; set; }
    public ContentType ContentType { get; set; }
}