using System;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Templates.Docx;
using Secretary.Documents.utils;

namespace Secretary.Documents.Creators.DocumentCreators;

public class DistantDocumentCreator: DocumentCreator<DistantData>
{
    public override string Create(DistantData data)
    {
        var doc = DocumentTemplatesStorage.Instance.GetDistantDocument();

        using var adapter = new DistantDocumentAdapter(doc);
        
        adapter.SetPersonName(data.Name);
        adapter.SetJobTitle(data.JobTitle);
        adapter.SetPeriod(data.Period);
        adapter.SetReason(data.Reason);
        adapter.SetSendingDay(DateTime.Now.ToString("dd.MM.yyyy"));

        var tempFile = new FileManager().CreateTempFile();

        adapter.SaveAs(tempFile);

        return tempFile;
    }
}