using HtmlAgilityPack;
using Xceed.Words.NET;

namespace Secretary.Documents;

public class DocumentTemplatesStorage
{
    public static DocumentTemplatesStorage Instance { get; }
    
    public static string Path = null!;

    static DocumentTemplatesStorage()
    {
        Instance = new DocumentTemplatesStorage();
    }

    public static void Initialize(string path)
    {
        Path = path;
    }

    public DocX GetTimeOffDocument()
    {
        return DocX.Load($"{Path}/docx/time-off.docx");
    }

    public DocX GetVacationDocument()
    {
        return DocX.Load($"{Path}/docx/vacation.docx");
    }

    public DocX GetDistantDocument()
    {
        return DocX.Load($"{Path}/docx/distant.docx");
    }

    public HtmlDocument GetTimeOffMessageTemplate()
    {
        var doc = new HtmlDocument();
        doc.Load($"{Path}/html/time-off.html");
        
        return doc;
    }

    public HtmlDocument GetVacationMessageTemplate()
    {
        var doc = new HtmlDocument();
        doc.Load($"{Path}/html/vacation.html");
        
        return doc;
    }

    public HtmlDocument GetDistantMessageTemplate()
    {
        var doc = new HtmlDocument();
        doc.Load($"{Path}/html/distant.html");
        
        return doc;
    }
    
    private DocumentTemplatesStorage() {}
}