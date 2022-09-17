using HtmlAgilityPack;
using Secretary.Configuration;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Creators.MessageCreators;

namespace Secretary.Documents.Tests.Creators.DocumentCreators;

public class VacationMessageCreatorTests
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
            Period = "с 26.12.1825 по 09.01.1826",
            Name = "Иван Иванов",
            JobTitle = "инженер-программист",
        };

        var html = new VacationMessageCreator().Create(data);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        
        var jobNode = doc.GetElementbyId("signature_job-title");
        Assert.That(jobNode.InnerHtml, Is.EqualTo("инженер-программист"));
        
        var nameNode = doc.GetElementbyId("signature_name");
        Assert.That(nameNode.InnerHtml, Is.EqualTo("Иван Иванов"));
        
        var node = doc.GetElementbyId("vacation_period");
        Assert.That(node.InnerHtml, Is.EqualTo("Прошу предоставить ежегодный оплачиваемый отпуск с 26.12.1825 по 09.01.1826, включительно."));
    }
}