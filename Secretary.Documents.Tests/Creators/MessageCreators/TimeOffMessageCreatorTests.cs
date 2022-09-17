using HtmlAgilityPack;
using Secretary.Configuration;
using Secretary.Documents.Creators.Data;
using Secretary.Documents.Creators.MessageCreators;

namespace Secretary.Documents.Tests.Creators.DocumentCreators;

public class TimeOffMessageCreatorTests
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
            Name = "Иван Иванов",
            JobTitle = "инженер-программист",
        };

        var html = new TimeOffMessageCreator().Create(data);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        
        var jobNode = doc.GetElementbyId("signature_job-title");
        Assert.That(jobNode.InnerHtml, Is.EqualTo("инженер-программист"));
        
        var nameNode = doc.GetElementbyId("signature_name");
        Assert.That(nameNode.InnerHtml, Is.EqualTo("Иван Иванов"));
        
        var periodNode = doc.GetElementbyId("time-off_period");
        Assert.That(periodNode.InnerHtml, Is.EqualTo("Прошу предоставить отгул 10.08.2022 с 9:00 до 13:00."));
        
        var reasonNode = doc.GetElementbyId("time-off_reason");
        Assert.That(reasonNode.InnerHtml, Is.EqualTo("<br>Причина: необходимо починить машину."));

        var workingOff = doc.GetElementbyId("time-off_working-off");
        Assert.That(workingOff.InnerHtml, Is.EqualTo("<br>Пропущенное время обязуюсь отработать."));
        
    }
}