using System.IO;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Templates.Html;

namespace Secretary.Documents.Creators.MessageCreators;

public class VacationMessageCreator: MessageCreator<VacationData>
{
    public override string Create(VacationData data)
    {
        var htmlDocument = DocumentTemplatesStorage.Instance.GetVacationMessageTemplate();

        var adapter = new VacationMessageAdapter(htmlDocument);
        
        adapter.SetPersonName(data.Name);
        adapter.SetJobTitle(data.JobTitle);
        adapter.SetDate(data.Period);

        var stringWriter = new StringWriter();

        adapter.SaveAs(stringWriter);

        return stringWriter.ToString();
    }
}