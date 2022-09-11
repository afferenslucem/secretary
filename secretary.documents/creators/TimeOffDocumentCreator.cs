using System;
using secretary.documents.templates.docx;
using secretary.documents.utils;

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

        var tempFile = new FileManager().CreateTempFile();

        adapter.SaveAs(tempFile);

        return tempFile;
    }
}