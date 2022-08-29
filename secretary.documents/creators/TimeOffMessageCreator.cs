using System;
using System.IO;
using secretary.documents.templates;
using secretary.documents.templates.docx;
using secretary.documents.templates.html;

namespace secretary.documents.creators;

public class TimeOffMessageCreator: ITimeOffCreator
{
    public string Create(TimeOffData data)
    {
        var htmlDocument = DocumentTemplatesStorage.Instance.GetTimeOffMessageTemplate();

        var adapter = new TimeOffMessageAdapter(htmlDocument);
        
        adapter.SetPersonName(data.Name);
        adapter.SetJobTitle(data.JobTitle);
        adapter.SetTimeOffPeriod(data.Period);
        adapter.SetReason(data.Reason);
        adapter.SetWorkingOff(data.WorkingOff);

        var stringWriter = new StringWriter();

        adapter.SaveAs(stringWriter);

        return stringWriter.ToString();
    }
}