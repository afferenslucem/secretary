using Secretary.Configuration;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Creators.DocumentCreators;
using Xceed.Words.NET;

namespace Secretary.Documents.Tests.Creators.DocumentCreators;

public class DistantCreatorTests
{
    [SetUp]
    public void Setup()
    {
        DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);
    }

    [Test]
    public void ShouldCreateInstance()
    {
        var data = new DistantData
        {
            Period = "c 10.08.2022 на неопределенный срок",
            Reason = "плохое самочувствие",
            Name = "Иванова Ивана Ивановича",
            JobTitle = "инженера-программиста"
        };

        var document = new DistantDocumentCreator().Create(data);
        using var result = DocX.Load(document);

        Assert.That(result.Paragraphs[2].Text, Is.EqualTo("от инженера-программиста"));
        Assert.That(result.Paragraphs[3].Text, Is.EqualTo("Иванова Ивана Ивановича"));
        Assert.That(result.Paragraphs[6].Text,
            Is.EqualTo("Прошу разрешить удаленную работу c 10.08.2022 на неопределенный срок."));
        Assert.That(result.Paragraphs[7].Text, Is.EqualTo("Причина: плохое самочувствие."));
        Assert.That(result.Paragraphs[10].Text, Is.EqualTo(DateTime.Now.ToString("dd.MM.yyyy")));
    }
}