using HtmlAgilityPack;
using Secretary.Configuration;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Creators.MessageCreators;

namespace Secretary.Documents.Tests.Creators.DocumentCreators;

public class DistantMessageCreatorTests
{
    [SetUp]
    public void Setup()
    {
        DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);
    }
    
    [Test]
    public void ShouldCreateInstance()
    {
        var data = new DistantData()
        {
            Period = "10.08.2022",
            Reason = "плохое самочувствие",
            Name = "Иван Иванов",
            JobTitle = "инженер-программист",
        };

        var html = new DistantMessageCreator().Create(data);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        
        var jobNode = doc.GetElementbyId("signature_job-title");
        Assert.That(jobNode.InnerHtml, Is.EqualTo("инженер-программист"));
        
        var nameNode = doc.GetElementbyId("signature_name");
        Assert.That(nameNode.InnerHtml, Is.EqualTo("Иван Иванов"));
        
        var periodNode = doc.GetElementbyId("distant_period");
        Assert.That(periodNode.InnerHtml, Is.EqualTo("Прошу разрешить удаленную работу 10.08.2022."));
        
        var reasonNode = doc.GetElementbyId("distant_reason");
        Assert.That(reasonNode.InnerHtml, Is.EqualTo("<br>Причина: плохое самочувствие."));
        
    }
}