using HtmlAgilityPack;
using secretary.configuration;
using secretary.documents.templates.docx;
using secretary.documents.templates.html;

namespace secretary.documents.tests.templates.html
{
    public class TimeOffMessageAdapterTests
    {
        public HtmlDocument Document = null!;

        [SetUp]
        public void Setup()
        {
            DocumentTemplatesStorage.Initialize(Config.Instance.TemplatesPath);
            this.Document = DocumentTemplatesStorage.Instance.GetTimeOffMessageTemplate();
        }

        [Test]
        public void ShouldCreateInstance()
        {
            var adapter = new TimeOffMessageAdapter(this.Document);
            
            Assert.Pass();
        }

        [Test]
        public void ShouldSetJobTitle()
        {
            var adapter = new TimeOffMessageAdapter(this.Document);
            
            adapter.SetJobTitle("инженер-программист");

            var node = this.Document.GetElementbyId("signature_job-title");
            
            Assert.That(node.InnerHtml, Is.EqualTo("инженер-программист"));
        }

        [Test]
        public void ShouldSetPersonName()
        {
            var adapter = new TimeOffMessageAdapter(this.Document);
            
            adapter.SetPersonName("Александр Пушкин");

            var node = this.Document.GetElementbyId("signature_name");
            
            Assert.That(node.InnerHtml, Is.EqualTo("Александр Пушкин"));
        }

        [Test]
        public void ShouldSetTimeOffDate()
        {
            var adapter = new TimeOffMessageAdapter(this.Document);
            
            adapter.SetTimeOffPeriod("26.12.1825");

            var node = this.Document.GetElementbyId("time-off_period");
            
            Assert.That(node.InnerHtml, Is.EqualTo("Прошу предоставить отгул 26.12.1825."));
        }

        [Test]
        public void ShouldSetReason()
        {
            var adapter = new TimeOffMessageAdapter(this.Document);
            
            adapter.SetReason("Нужно съездить на восстание");

            var node = this.Document.GetElementbyId("time-off_reason");
            
            Assert.That(node.InnerHtml, Is.EqualTo("<br>Причина: Нужно съездить на восстание."));
        }

        [Test]
        public void ShouldDeleteReasonIfSetNull()
        {
            var adapter = new TimeOffMessageAdapter(this.Document);
            
            adapter.SetReason(null);

            var node = this.Document.GetElementbyId("time-off_reason");
            
            Assert.IsNull(node);
        }

        [Test]
        public void ShouldSetWorkingOff()
        {
            var adapter = new TimeOffMessageAdapter(this.Document);
            
            adapter.SetWorkingOff("Отработаю в ссылке");

            var node = this.Document.GetElementbyId("time-off_working-off");
            
            Assert.That(node.InnerHtml, Is.EqualTo("<br>Отработаю в ссылке."));
        }

        [Test]
        public void ShouldDeleteWorkingOffIfSetNull()
        {
            var adapter = new TimeOffMessageAdapter(this.Document);
            
            adapter.SetWorkingOff(null);

            var node = this.Document.GetElementbyId("time-off_working-off");
            
            Assert.IsNull(node);
        }

        [Test]
        public void ShouldSave()
        {
            var adapter = new TimeOffMessageAdapter(this.Document);
            
            adapter.SetWorkingOff(null);

            var writer = new StringWriter();
            
            adapter.SaveAs(writer);
            
            Assert.That(writer.ToString().Length, Is.GreaterThan(0));
        }
    }
}