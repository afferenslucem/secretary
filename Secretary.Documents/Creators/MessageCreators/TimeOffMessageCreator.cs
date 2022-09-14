using System.IO;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Templates.Html;

namespace Secretary.Documents.Creators.MessageCreators;

public class TimeOffMessageCreator: MessageCreator<TimeOffData>
{
    public override string Create(TimeOffData data)
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