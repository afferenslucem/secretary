using System;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Templates.Docx;
using Secretary.Documents.utils;

namespace Secretary.Documents.Creators.DocumentCreators;

public class VacationDocumentCreator: DocumentCreator<VacationData>
{
    public override string Create(VacationData data)
    {
        var doc = DocumentTemplatesStorage.Instance.GetVacationDocument();

        using var adapter = new VacationDocumentAdapter(doc);
        
        adapter.SetPersonName(data.Name);
        adapter.SetJobTitle(data.JobTitle);
        adapter.SetPeriod(data.Period);
        adapter.SetSendingDay(DateTime.Now.ToString("dd.MM.yyyy"));

        var tempFile = new FileManager().CreateTempFile();

        adapter.SaveAs(tempFile);

        return tempFile;
    }
}