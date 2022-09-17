using System.IO;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Templates.Html;

namespace Secretary.Documents.Creators.MessageCreators;

public class DistantMessageCreator: MessageCreator<DistantData>
{
    public override string Create(DistantData data)
    {
        var htmlDocument = DocumentTemplatesStorage.Instance.GetDistantMessageTemplate();

        var adapter = new DistantMessageAdapter(htmlDocument);
        
        adapter.SetPersonName(data.Name);
        adapter.SetJobTitle(data.JobTitle);
        adapter.SetTimeOffPeriod(data.Period);
        adapter.SetReason(data.Reason);

        var stringWriter = new StringWriter();

        adapter.SaveAs(stringWriter);

        return stringWriter.ToString();
    }
}