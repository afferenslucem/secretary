using System;
using System.IO;
using secretary.documents.templates.docx;

namespace secretary.documents.creators;

public class TimeOffDocumentCreator: ITimeOffCreator
{
    public string Create(TimeOffData data)
    {
        var doc = DocumentTemplatesStorage.Instance.GetTimeOffDocument();

        using var adapter = new TimeOffDocumentAdapter(doc);
        
        adapter.SetPersonName(data.Name);
        adapter.SetJobTitle(data.JobTitle);
        adapter.SetTimeOffPeriod(data.Period);
        adapter.SetReason(data.Reason);
        adapter.SetWorkingOff(data.WorkingOff);
        adapter.SetSendingDay(DateTime.Now.ToString("dd.MM.yyyy"));

        var tempFile = this.CreateTempFile();

        adapter.SaveAs(tempFile);

        return tempFile;
    }

    private string CreateTempFile()
    {
        var tempFile = Path.GetTempFileName();
        File.Move(tempFile, tempFile.Replace(".tmp", ".docx"));
        tempFile = tempFile.Replace(".tmp", ".docx");

        return tempFile;
    }
}