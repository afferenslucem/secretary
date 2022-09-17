using HtmlAgilityPack;
using Secretary.Configuration;
using Secretary.Documents.Templates.Html;

namespace Secretary.Documents.Tests.Templates.Html
{
    public class VacationMessageAdapterTests
    {
        public HtmlDocument Document = null!;

        [SetUp]
        public void Setup()
        {
            DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);
            Document = DocumentTemplatesStorage.Instance.GetVacationMessageTemplate();
        }

        [Test]
        public void ShouldCreateInstance()
        {
            var adapter = new VacationMessageAdapter(Document);
            
            Assert.Pass();
        }

        [Test]
        public void ShouldSetJobTitle()
        {
            var adapter = new VacationMessageAdapter(Document);
            
            adapter.SetJobTitle("инженер-программист");

            var node = Document.GetElementbyId("signature_job-title");
            
            Assert.That(node.InnerHtml, Is.EqualTo("инженер-программист"));
        }

        [Test]
        public void ShouldSetPersonName()
        {
            var adapter = new VacationMessageAdapter(Document);
            
            adapter.SetPersonName("Александр Пушкин");

            var node = Document.GetElementbyId("signature_name");
            
            Assert.That(node.InnerHtml, Is.EqualTo("Александр Пушкин"));
        }

        [Test]
        public void ShouldSetPeriodDateWithoutExtraPoint()
        {
            var adapter = new VacationMessageAdapter(Document);
            
            adapter.SetDate("с 26.12.1825 по 09.01.1826");

            var node = Document.GetElementbyId("vacation_period");
            
            Assert.That(node.InnerHtml, Is.EqualTo("Прошу предоставить ежегодный оплачиваемый отпуск с 26.12.1825 по 09.01.1826, включительно."));
        }

        [Test]
        public void ShouldSave()
        {
            var adapter = new VacationMessageAdapter(Document);
            
            var writer = new StringWriter();
            
            adapter.SaveAs(writer);
            
            Assert.That(writer.ToString().Length, Is.GreaterThan(0));
        }
    }
}