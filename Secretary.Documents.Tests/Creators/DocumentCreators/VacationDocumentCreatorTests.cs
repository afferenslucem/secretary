using Secretary.Configuration;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Creators.DocumentCreators;
using Xceed.Words.NET;

namespace Secretary.Documents.Tests.Creators.DocumentCreators;

public class VacationDocumentCreatorTests
{
    [SetUp]
    public void Setup()
    {
        DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);
    }
    
    [Test]
    public void ShouldCreateInstance()
    {
        var data = new VacationData()
        {
            Period = "с 10.08.2022 по 23.08.2022",
            Name = "Иванова Ивана Ивановича",
            JobTitle = "инженера-программиста",
        };

        var document = new VacationDocumentCreator().Create(data);
        using var result = DocX.Load(document);
        
        Assert.That(result.Paragraphs[2].Text, Is.EqualTo("от инженера-программиста"));
        Assert.That(result.Paragraphs[3].Text, Is.EqualTo("Иванова Ивана Ивановича"));
        Assert.That(result.Paragraphs[6].Text, Is.EqualTo("Прошу предоставить ежегодный оплачиваемый отпуск с 10.08.2022 по 23.08.2022, включительно."));
        Assert.That(result.Paragraphs[8].Text, Is.EqualTo(DateTime.Now.ToString("dd.MM.yyyy")));
    }
}