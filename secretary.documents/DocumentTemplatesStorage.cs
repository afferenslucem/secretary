using HtmlAgilityPack;
using Xceed.Words.NET;

namespace secretary.documents;

public class DocumentTemplatesStorage
{
    public static DocumentTemplatesStorage Instance { get; private set; }
    
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

    public HtmlDocument GetTimeOffMessageTemplate()
    {
        var doc = new HtmlDocument();
        doc.Load($"{Path}/html/time-off.html");
        
        return doc;
    }
    
    private DocumentTemplatesStorage() {}
}