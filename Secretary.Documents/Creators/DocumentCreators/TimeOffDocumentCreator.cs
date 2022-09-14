using System;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Templates.Docx;
using Secretary.Documents.utils;

namespace Secretary.Documents.Creators.DocumentCreators;

public class TimeOffDocumentCreator: DocumentCreator<TimeOffData>
{
    public override string Create(TimeOffData data)
    {
        var doc = DocumentTemplatesStorage.Instance.GetTimeOffDocument();

        using var adapter = new TimeOffDocumentAdapter(doc);
        
        adapter.SetPersonName(data.Name);
        adapter.SetJobTitle(data.JobTitle);
        adapter.SetPeriod(data.Period);
        adapter.SetReason(data.Reason);
        adapter.SetWorkingOff(data.WorkingOff);
        adapter.SetSendingDay(DateTime.Now.ToString("dd.MM.yyyy"));

        var tempFile = new FileManager().CreateTempFile();

        adapter.SaveAs(tempFile);

        return tempFile;
    }
}