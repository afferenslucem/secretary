using System;
using System.Linq;
using Secretary.Documents.utils;
using Xceed.Words.NET;

namespace Secretary.Documents.Templates.Docx;

public class DocumentAdapter : IDisposable
{
    protected readonly DocX Document;

    public DocumentAdapter(DocX document)
    {
        Document = document;
    }

    public void SetJobTitle(string value)
    {
        SetValue(Placeholders.JobTitle, value);
    }

    public void SetPersonName(string value)
    {
        SetValue(Placeholders.PersonName, value);
    }

    public void SetSendingDay(string value)
    {
        SetValue(Placeholders.SendingDay, value);
    }

    public void SaveAs(string name)
    {
        Document.SaveAs(name);
    }

    public void Dispose()
    {
        Document?.Dispose();
    }

    protected void SetValue(string placeholder, string? value)
    {
        var target = Document.Paragraphs.FirstOrDefault(paragraph => paragraph.Text.Contains(placeholder));

        if (target == null)
        {
            throw new Exception("Incorrect placeholder");
        }

        if (value == null)
        {
            target.RemoveText(0);
            target.Hide();
        }
        else
        {
            target.ReplaceText(placeholder, value);   
        }
    }
}