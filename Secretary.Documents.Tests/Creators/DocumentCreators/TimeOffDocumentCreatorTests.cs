using Secretary.Configuration;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Creators.DocumentCreators;
using Xceed.Words.NET;

namespace Secretary.Documents.Tests.Creators.DocumentCreators;

public class TimeOffDocumentCreatorTests
{
    [SetUp]
    public void Setup()
    {
        DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);
    }
    
    [Test]
    public void ShouldCreateInstance()
    {
        var data = new TimeOffData()
        {
            Period = "10.08.2022 с 9:00 до 13:00",
            Reason = "необходимо починить машину",
            WorkingOff = "Пропущенное время обязуюсь отработать",
            Name = "Иванова Ивана Ивановича",
            JobTitle = "инженера-программиста",
        };

        var document = new TimeOffDocumentCreator().Create(data);
        using var result = DocX.Load(document);
        
        Assert.That(result.Paragraphs[2].Text, Is.EqualTo("от инженера-программиста"));
        Assert.That(result.Paragraphs[3].Text, Is.EqualTo("Иванова Ивана Ивановича"));
        Assert.That(result.Paragraphs[6].Text, Is.EqualTo("Прошу предоставить отгул 10.08.2022 с 9:00 до 13:00."));
        Assert.That(result.Paragraphs[7].Text, Is.EqualTo("Причина: необходимо починить машину."));
        Assert.That(result.Paragraphs[8].Text, Is.EqualTo("Пропущенное время обязуюсь отработать."));
        Assert.That(result.Paragraphs[10].Text, Is.EqualTo(DateTime.Now.ToString("dd.MM.yyyy")));
    }
}