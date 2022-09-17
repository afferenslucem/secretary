using System.IO;
using HtmlAgilityPack;

namespace Secretary.Documents.Templates.Html;

public class MessageAdapter
{
    protected HtmlDocument Doc;

    public MessageAdapter(HtmlDocument doc)
    {
        this.Doc = doc;
    }

    public void SetJobTitle(string value)
    {
        SetValue("signature_job-title", Placeholders.JobTitle, value);
    }

    public void SetPersonName(string value)
    {
        SetValue("signature_name", Placeholders.PersonName, value);
    }

    public void SaveAs(TextWriter writer)
    {
        this.Doc.Save(writer);
    }

    protected void SetValue(string id, string placehoder, string? value)
    {
        var reason = Doc.GetElementbyId(id);

        var parent = reason.ParentNode;
             
        if (value == null)
        {
            parent.RemoveChild(reason);
        }
        else
        {
            var replaced = reason.OuterHtml.Replace(placehoder, value);

            var newNode = HtmlNode.CreateNode(replaced);

            parent.ReplaceChild(newNode, reason);
        }
    }
}